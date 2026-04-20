using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Entities
{
    public class BookModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}