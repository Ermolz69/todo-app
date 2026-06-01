using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(category => category.Color)
            .HasMaxLength(32);

        builder.Property(category => category.Icon)
            .HasMaxLength(64);

        builder.HasOne(category => category.User)
            .WithMany(user => user.Categories)
            .HasForeignKey(category => category.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(category => category.TaskItems)
            .WithOne(task => task.Category)
            .HasForeignKey(task => task.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(category => new { category.UserId, category.Name });
    }
}
