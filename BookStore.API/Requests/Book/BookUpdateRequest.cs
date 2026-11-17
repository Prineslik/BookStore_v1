using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.API.Requests.Book
{
    public record BookUpdateRequest(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        int StockQuantity,
        List<Guid> OwnerIds,
        List<Guid> AuthorIds
    );
}
