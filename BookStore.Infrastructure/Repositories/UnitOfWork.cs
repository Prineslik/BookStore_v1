using BookStore.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookStoreDbContext _context;
        public UnitOfWork(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);   
        }
    }
}
