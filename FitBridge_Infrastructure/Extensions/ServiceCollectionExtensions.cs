using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Interfaces.Utils.Seeding;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Infrastructure.Persistence;
using FitBridge_Infrastructure.Seeder;
using FitBridge_Infrastructure.Services;
using FitBridge_Infrastructure.Services.Implements;
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
using StackExchange.Redis;
using System.Threading.Channels;

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

            services.Configure<FirebaseSettings>(configuration.GetSection(FirebaseSettings.SectionName));
            services.Configure<PayOSSettings>(configuration.GetSection(PayOSSettings.SectionName));
            services.Configure<NotificationSettings>(configuration.GetSection(NotificationSettings.SectionName));

            var channel = Channel.CreateUnbounded<NotificationMessage>(new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = false
            });

            services.AddSignalR();
            services.AddSingleton<ChannelWriter<NotificationMessage>>(channel.Writer);
            services.AddSingleton<ChannelReader<NotificationMessage>>(channel.Reader);
            services.AddSingleton<FirebaseService>();
            services.AddSingleton<PushNotificationService>();
            services.AddSingleton<NotificationConnectionManager>();
            services.AddSingleton<NotificationHandshakeManager>();
            services.AddSingleton<NotificationStorageService>();

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
        }
    }
}