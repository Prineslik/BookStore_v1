using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Books.DTO
{
    public record BookShortDTO(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        int StockQuantity,
        DateTime CreateDate,
        DateTime? UpdateDate
    );
}
