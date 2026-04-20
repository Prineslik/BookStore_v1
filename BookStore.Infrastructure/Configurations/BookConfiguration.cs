using BookStore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<BookEntity>
    {
        public void Configure(EntityTypeBuilder<BookEntity> builder)
        {
            builder.HasKey(b => b.Id);

            builder
                .Property(b => b.Title)
                .HasMaxLength(BookEntity.MAX_TITLE_LENGTH)
                .IsRequired();

            builder
                .Property(b => b.Description)
                .IsRequired();

            builder
                .Property(b => b.Price)
                .IsRequired();
        }
    }
}
