using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Core.Entities
{
    public class BookEntity
    {
        public const int MAX_TITLE_LENGTH = 250;

        private BookEntity()
        {
        }

        private BookEntity(Guid id, string title, string description, decimal price)
        {
            Id = id;
            Title = title;
            Description = description;
            Price = price;
        }

        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }

        public static (BookEntity Book, string Error) Create(Guid id, string title, string description, decimal price)
        {
            var error = string.Empty;

            if (string.IsNullOrEmpty(title) || title.Length > MAX_TITLE_LENGTH)
            {
                error = "Title can not be empty or longer than 250";
            }

            price = Math.Round(price, 2);

            var book = new BookEntity(id, title, description, price);

            return (book, error);
        }
    }
}
