using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}
