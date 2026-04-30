using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<BookModel>
    {
        public void Configure(EntityTypeBuilder<BookModel> builder)
        {
            builder.HasKey(b => b.Id);

            builder
                .Property(b => b.Title)
                .HasMaxLength(250/*BookEntity.MAX_TITLE_LENGTH*/)
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
