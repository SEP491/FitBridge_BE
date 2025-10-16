using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");
        builder.Property(e => e.Content).IsRequired(true);
        builder.Property(e => e.MessageType).IsRequired(true);
        builder.Property(e => e.Metadata).IsRequired(false);
        builder.Property(e => e.DeletedAt).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.ReplyToMessageId).IsRequired(false);
        builder.Property(e => e.IsEdited).HasDefaultValue(false);

        builder.HasOne(e => e.Sender).WithMany(e => e.Messages).HasForeignKey(e => e.SenderId);
        builder.HasOne(e => e.ReplyToMessage).WithMany(e => e.ReplyToMessages).HasForeignKey(e => e.ReplyToMessageId);
        builder.HasOne(e => e.Conversation).WithMany(e => e.Messages).HasForeignKey(e => e.ConversationId);
    }
}
