using FitBridge_Application.Interfaces.Utils.Seeding;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Infrastructure.Persistence;
using FitBridge_Infrastructure.Seeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<FitBridgeDbContext>();

            services.AddScoped<IIdentitySeeder, IdentitySeeder>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}