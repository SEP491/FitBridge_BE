using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
{
    public void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        builder.ToTable("ConversationMembers");
        // Composite unique index for UserId and ConversationId
        builder.HasIndex(e => new { e.UserId, e.ConversationId }).IsUnique();
        // Property configurations
        builder.Property(e => e.ConversationId).IsRequired(true);
        builder.Property(e => e.UserId).IsRequired(true);
        builder.Property(e => e.CustomTitle).IsRequired(true);
        builder.Property(e => e.ConversationImage).IsRequired(false);
        builder.Property(e => e.LastMessageId).IsRequired(false);
        builder.Property(e => e.LastReadAt).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        // Relationship configurations
        builder.HasOne(e => e.User)
            .WithMany(e => e.ConversationMembers)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Conversation)
            .WithMany(e => e.ConversationMembers)
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.LastReadMessage)
            .WithMany()
            .HasForeignKey(e => e.LastMessageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.Messages)
            .WithOne(e => e.Sender)
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.MessageStatuses)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}