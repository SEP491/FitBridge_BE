using FitBridge_Application.Interfaces.Utils.Seeding;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Infrastructure.Persistence;
using FitBridge_Infrastructure.Seeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Infrastructure.Services.Implements;
using FitBridge_Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Infrastructure.Utils;
using FitBridge_Infrastructure.Services.Templating;
using FitBridge_Application.Configurations;
using Microsoft.Extensions.Options;

namespace FitBridge_Infrastructure.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<FitBridgeDbContext>(options =>
                options
                    .UseNpgsql(configuration.GetConnectionString("FitBridgeDb"))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors());

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                // SignIn settings
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<FitBridgeDbContext>()
                .AddDefaultTokenProviders();
            
            services.Configure<PayOSSettings>(configuration.GetSection(PayOSSettings.SectionName));
    

            services.AddScoped<IIdentitySeeder, IdentitySeeder>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserTokenService, UserTokenService>();
            services.AddScoped<IApplicationUserService, ApplicationUserService>();
            services.AddScoped<IUserUtil, UserUtil>();
            services.AddScoped<ITemplatingService, TemplatingService>();
            services.AddScoped<IPayOSService, PayOSService>();
            
    

        }
    }
}