using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class UserBookModel
    {
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }

        public UserModel User { get; set; } = null!;
        public BookModel Book { get; set; } = null!;
    }
}
