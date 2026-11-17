using BookStore.Application.Contracts.Authors.DTO;
using BookStore.Application.Contracts.Users.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Books.DTO
{
    public record BookDetailsDTO(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        int StockQuantity,
        DateTime CreateDate,
        DateTime? UpdateDate,
        List<OwnerShortDTO> Owners,
        List<AuthorShortDTO> Authors);
}
