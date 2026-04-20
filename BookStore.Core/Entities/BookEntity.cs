using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Core.Entities
{
    public class BookEntity
    {
        public const int MAX_TITLE_LENGTH = 250;

        public BookEntity()
        {
        }

        private BookEntity(Guid id, string title, string description, decimal price)
        {
            Id = id;
            Title = title;
            Description = description;
            Price = price;
        }

        public Guid Id { get; set; }
        public string Title { get; } = string.Empty;
        public string Description { get; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public static (BookEntity Book, string Error) Create(Guid id, string title, string description, decimal price)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(title) || title.Length > MAX_TITLE_LENGTH)
            {
                error = "Title can not be empty or longer than 250";
            }

            var book = new BookEntity(id, title, description, price);

            return (book, error);
        }
    }
}
