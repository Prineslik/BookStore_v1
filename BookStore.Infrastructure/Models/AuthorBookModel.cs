using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class AuthorBookModel
    {
        public Guid AuthorId { get; set; }
        public Guid BookId { get; set; }

        public AuthorModel Author { get; set; } = null!;
        public BookModel Book { get; set; } = null!;
    }
}
