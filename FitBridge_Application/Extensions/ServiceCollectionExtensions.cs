using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitBridge_Application.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static void AddApplications(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

            services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = configuration["AutoMapperLicenseKey"];
            }, applicationAssembly);

            services.AddMediatR(cfg =>
            {
                cfg.LicenseKey = configuration["AutoMapperLicenseKey"];
                cfg.RegisterServicesFromAssembly(applicationAssembly);
            });
        }
    }
}