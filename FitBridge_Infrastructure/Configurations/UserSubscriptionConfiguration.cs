using System;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Enums.SubscriptionPlans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace FitBridge_Infrastructure.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscriptions");
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.SubscriptionPlanId).IsRequired();
        builder.Property(e => e.StartDate).IsRequired();
        builder.Property(e => e.EndDate).IsRequired();
        builder.Property(e => e.LimitUsage).IsRequired(false);
        builder.Property(e => e.CurrentUsage).HasDefaultValue(0);
        builder.Property(e => e.Status).HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<SubScriptionStatus>(s))
        .HasDefaultValue(SubScriptionStatus.Active);
        builder.HasOne(e => e.User).WithMany(e => e.UserSubscriptions).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.SubscriptionPlansInformation).WithMany(e => e.UserSubscriptions).HasForeignKey(e => e.SubscriptionPlanId).OnDelete(DeleteBehavior.SetNull);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
    }
}
