using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Books.Commands
{
    public record BookUpdateCommand(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        int StockQuantity,
        List<Guid> OwnerIds,
        List<Guid> AuthorIds
     );
}
