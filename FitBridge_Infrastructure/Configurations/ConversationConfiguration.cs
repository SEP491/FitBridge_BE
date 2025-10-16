using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");
        builder.Property(e => e.Title).IsRequired(false);
        builder.Property(e => e.LastMessageId).IsRequired(false);
        builder.Property(e => e.LastMessageContent).IsRequired(false);
        builder.Property(e => e.LastMessageType).IsRequired(false);
        builder.Property(e => e.LastMessageSenderId).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.LastMessageId).IsRequired(true);
        builder.Property(e => e.LastMessageSenderId).IsRequired(true);
        
        builder.HasOne(e => e.LastMessageSender).WithMany(e => e.Conversations).HasForeignKey(e => e.LastMessageSenderId);
    }
}
