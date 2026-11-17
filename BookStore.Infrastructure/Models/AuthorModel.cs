using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class AuthorModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? PhotoUrl { get; set; }
          
        public Guid? UserId { get; set; }
        public UserModel? User { get; set; }
        
        public List<AuthorBookModel> AuthorBookModels { get; set; } = [];
    }
}
