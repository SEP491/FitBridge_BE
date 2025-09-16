using Asp.Versioning;
using FitBridge_API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace FitBridge_API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static void AddPresentation(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            //Add Swagger
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "FitBridge's API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                option.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

                //JWT Config
                option.DescribeAllParametersInCamelCase();
                option.ResolveConflictingActions(conf => conf.First());     // duplicate API name if any, ex: Get() & Get(string id)
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            // Add Controllers with Endpoints
            builder.Services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtAccessTokenSettings:Secret"]!)),
                    ValidIssuer = builder.Configuration["JwtAccessTokenSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtAccessTokenSettings:Audience"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(3) //Validator will still consider the token validate if it has expired 3 seconds ago, this is to prevent in case the request takes some time to reach to the api
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        // If the request is for our hub
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hub/signaling") || path.StartsWithSegments("/hub/messaging")))
                        {
                            context.Token = accessToken;
                            Console.WriteLine($"[JWT] Token extracted for SignalR: {accessToken.ToString().Substring(0, Math.Min(20, accessToken.ToString().Length))}...");
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"[JWT] Token validated successfully for user: {context.Principal?.Identity?.Name}");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[JWT] Authentication failed: {context.Exception?.Message}");
                        Console.WriteLine($"[JWT] Token: {context.Request.Headers.Authorization.FirstOrDefault()}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"[JWT] Authentication challenged: {context.Error} - {context.ErrorDescription}");
                        return Task.CompletedTask;
                    }
                };
            });

            // Add Authorization
            builder.Services.AddAuthorization(options =>
            {
                var requireAuthPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                options.DefaultPolicy = requireAuthPolicy;
            });

            // Config Routing on Urls
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("Location");
                });

                options.AddPolicy("AllowAllOrigin", policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders("Location");
                });
            });

            // tell swagger to support minimal apis, which the Identity apis are.
            builder.Services.AddEndpointsApiExplorer();

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All;
            });

            // Add API Versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(

                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new MediaTypeApiVersionReader("X-Version"));
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }
    }
}