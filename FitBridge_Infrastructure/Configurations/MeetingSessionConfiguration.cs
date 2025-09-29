using System;
using FitBridge_Domain.Entities.Meetings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class MeetingSessionConfiguration : IEntityTypeConfiguration<MeetingSession>
{
    public void Configure(EntityTypeBuilder<MeetingSession> builder)
    {
        builder.ToTable("MeetingSessions");
        builder.Property(e => e.UserOneId).IsRequired(true);
        builder.Property(e => e.UserTwoId).IsRequired(true);

        builder.HasOne(e => e.UserOne)
        .WithMany().HasForeignKey(e => e.UserOneId)
        .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.UserTwo)
        .WithMany().HasForeignKey(e => e.UserTwoId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
