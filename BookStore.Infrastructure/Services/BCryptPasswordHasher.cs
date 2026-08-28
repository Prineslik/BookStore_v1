using BookStore.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Services
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }
    }
}
