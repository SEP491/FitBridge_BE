using dotAPNS;
using dotAPNS.AspNetCore;
using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Interfaces.Utils.Seeding;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Infrastructure.Jobs.Coupons;
using FitBridge_Infrastructure.Persistence;
using FitBridge_Infrastructure.Services.Uploads;
using FitBridge_Infrastructure.Seeder;
using FitBridge_Infrastructure.Services;
using FitBridge_Infrastructure.Services.Implements;
using FitBridge_Infrastructure.Services.Jobs;
using FitBridge_Infrastructure.Services.Notifications;
using FitBridge_Infrastructure.Services.Notifications.Helpers;
using FitBridge_Infrastructure.Services.Templating;
using FitBridge_Infrastructure.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using StackExchange.Redis;
using System.Threading.Channels;
using Appwrite;
using Appwrite.Services;
using FitBridge_Infrastructure.Services.Meetings.Helpers;
using FitBridge_Application.Interfaces.Services.Messaging;
using FitBridge_Infrastructure.Services.Messaging;

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

            services.AddSingleton<IConnectionMultiplexer>(config =>
            {
                ArgumentException.ThrowIfNullOrEmpty(configuration.GetSection("Redis:ConnectionString").Value);
                var connectionString = configuration.GetSection("Redis:ConnectionString").Value!;
                return ConnectionMultiplexer.Connect(connectionString);
            });
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<AppWriteSettings>>().Value;
                var client = new Client()
                    .SetEndpoint(settings.EndPoint)
                    .SetProject(settings.ProjectId)
                    .SetKey(settings.APIKey);
                return client;
            });

            // Register Appwrite Storage Service
            services.AddSingleton(sp =>
            {
                var client = sp.GetRequiredService<Client>();
                return new Storage(client);
            });

            services.Configure<FirebaseSettings>(configuration.GetSection(FirebaseSettings.SectionName));
            services.Configure<PayOSSettings>(configuration.GetSection(PayOSSettings.SectionName));
            services.Configure<NotificationSettings>(configuration.GetSection(NotificationSettings.SectionName));
            services.Configure<AppWriteSettings>(configuration.GetSection(AppWriteSettings.SectionName));
            services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
            services.Configure<MeetingSettings>(configuration.GetSection(MeetingSettings.SectionName));
            services.Configure<AhamoveSettings>(configuration.GetSection(AhamoveSettings.SectionName));
            var channel = Channel.CreateUnbounded<NotificationMessage>(new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = false
            });

            services.AddQuartz(q =>
            {
                q.UseSimpleTypeLoader();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                var disableExpiredCouponsJobKey = new JobKey("DisableExpiredCouponsJob");

                q.AddJob<DisableExpiredCouponsJob>(opts => opts.WithIdentity(disableExpiredCouponsJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(disableExpiredCouponsJobKey)
                    .WithIdentity("DisableExpiredCouponsTrigger")
                    .WithCronSchedule("0 0 0 * * ?") // Run daily at 00:00:00
                    .WithDescription("Disables expired coupons daily at midnight"));

                var enableStartingCouponsJobKey = new JobKey("EnableStartingCouponsJob");
                q.AddJob<EnableStartingCouponsJob>(opts => opts.WithIdentity(enableStartingCouponsJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(enableStartingCouponsJobKey)
                    .WithIdentity("EnableStartingCouponsTrigger")
                    .WithCronSchedule("0 0 0 * * ?") // Run daily at 00:00:00
                    .WithDescription("Enables starting coupons daily at midnight"));
                q.UsePersistentStore(store =>
                {
                    store.UsePostgres(postgres =>
                    {
                        postgres.ConnectionString = configuration.GetConnectionString("FitbridgeDb");
                        postgres.TablePrefix = "public.qrtz_";
                    });
                    store.UseNewtonsoftJsonSerializer();
                    store.UseClustering();
                });
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            services.AddSignalR()
                    .AddStackExchangeRedis(options =>
                    {
                        options.ConnectionFactory = async writer =>
                        {
                            var connection = await ConnectionMultiplexer
                                .ConnectAsync(configuration["Redis:ConnectionString"]!, writer);

                            connection.ConnectionFailed += (_, e) =>
                            {
                                Console.WriteLine("Connection to Redis failed.");
                            };

                            if (!connection.IsConnected)
                            {
                                Console.WriteLine("Did not connect to Redis.");
                            }
                            return connection;
                        };
                        options.Configuration.ChannelPrefix = configuration["Redis:SignalRChannel"]!; // not needed if using one signlar app
                        options.Configuration.DefaultDatabase = configuration.GetValue<int>("Redis:SignalrRedisDatabase");
                    });
            services.AddApns();

            services.AddSingleton<ChannelWriter<NotificationMessage>>(channel.Writer);
            services.AddSingleton<ChannelReader<NotificationMessage>>(channel.Reader);
            services.AddSingleton<FirebaseService>();
            services.AddSingleton<PushNotificationService>();
            services.AddSingleton<NotificationConnectionManager>();
            services.AddSingleton<NotificationHandshakeManager>();
            services.AddSingleton<SessionManager>();

            services.AddScoped<IMessagingHubService, MessagingHubService>();
            services.AddScoped<INotificationService, NotificationsService>();
            services.AddScoped<IIdentitySeeder, IdentitySeeder>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserTokenService, UserTokenService>();
            services.AddScoped<IApplicationUserService, ApplicationUserService>();
            services.AddScoped<IUserUtil, UserUtil>();
            services.AddScoped<TemplatingService>();
            services.AddScoped<IPayOSService, PayOSService>();
            services.AddScoped<ITransactionService, TransactionsService>();
            services.AddHostedService<NotificationsBackgroundService>();
            services.AddScoped<IScheduleJobServices, ScheduleJobServices>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IAppleNotificationServerService, AppleNotificationServerService>();
            services.AddScoped<SystemConfigurationService>();
            services.AddScoped<SubscriptionService>();
            services.AddScoped<BookingService>();
            
            // Register HttpClient for Ahamove Service
            services.AddHttpClient<IAhamoveService, AhamoveService>();
        }
    }
}