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
        builder.Property(e => e.ConversationId).IsRequired(true);
        builder.Property(e => e.UserId).IsRequired(true);
        builder.Property(e => e.CustomTitle).IsRequired(false);
        builder.Property(e => e.ConversationImage).IsRequired(false);
        builder.Property(e => e.LastMessageId).IsRequired(false);
        builder.Property(e => e.LastReadAt).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.User).WithMany(e => e.ConversationMembers).HasForeignKey(e => e.UserId);
        builder.HasOne(e => e.Conversation).WithMany(e => e.ConversationMembers).HasForeignKey(e => e.ConversationId);
    }
}
