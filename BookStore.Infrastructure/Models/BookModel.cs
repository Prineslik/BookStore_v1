using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class BookModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public List<UserBookModel> UserBookModels { get; set; } = null!;
        public List<AuthorBookModel> AuthorBookModels { get; set; } = null!;
    }
}