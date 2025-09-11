using System;
using FitBridge_Domain.Entities.Blogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class BlogConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.ToTable("Blogs");
        builder.Property(e => e.Title).IsRequired(true);
        builder.Property(e => e.Content).IsRequired(true);
        builder.Property(e => e.AuthorId).IsRequired(true);

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        
        builder.HasOne(e => e.Author).WithMany(e => e.Blogs).HasForeignKey(e => e.AuthorId);
    }
}
