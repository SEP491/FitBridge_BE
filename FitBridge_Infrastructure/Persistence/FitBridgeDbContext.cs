using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitBridge_Infrastructure.Persistence
{
    public class FitBridgeDbContext(DbContextOptions<FitBridgeDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ApplyBaseEntityConfigurationToDerivedClass(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitBridgeDbContext).Assembly);
        }

        /// <summary>
        /// Configuring the base entity
        /// </summary>
        /// <param name="modelBuilder"></param>
        private static void ApplyBaseEntityConfigurationToDerivedClass(ModelBuilder modelBuilder)
        {
            var converter = new UtcToLocalDateTimeConverter();
            var clrTypes = modelBuilder.Model.GetEntityTypes()
                .Where(entityType =>
                    typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)
                    && entityType.ClrType != typeof(BaseEntity))
                .Select(entityType => entityType.ClrType);

            foreach (var clrType in clrTypes)
            {
                // Configure Id
                modelBuilder.Entity(clrType)
                    .Property(nameof(BaseEntity.Id))
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn();

                var properties = clrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

                foreach (var property in properties)
                {
                    modelBuilder.Entity(clrType)
                        .Property(property.Name)
                        .HasConversion(converter);
                }
            }
        }
    }
}