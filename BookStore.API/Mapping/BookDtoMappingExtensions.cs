using BookStore.API.Requests.Book;
using BookStore.API.Responses.Book;
using BookStore.Application.Contracts.Books.Commands;
using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Common;

namespace BookStore.API.Mapping
{
    public static class BookDtoMappingExtensions
    {
        public static List<BookShortResponse> ToResponse(this List<BookShortDTO> listDTOs)
            => listDTOs.Select(b => new BookShortResponse(
                b.Id,
                b.Title,
                b.Description,
                b.Price,
                b.StockQuantity,
                b.CreateDate,
                b.UpdateDate))
            .ToList();

        public static BookDetailsResponse ToResponse(this BookDetailsDTO detailsDTO)
            => new BookDetailsResponse(
                detailsDTO.Id,
                detailsDTO.Title,
                detailsDTO.Description,
                detailsDTO.Price,
                detailsDTO.StockQuantity,
                detailsDTO.CreateDate,
                detailsDTO.UpdateDate,
                detailsDTO.Owners.ToResponse(),
                detailsDTO.Authors.ToResponse());

        public static PagedResult<BookShortResponse> ToResponse(this PagedResult<BookShortDTO> pagedResult)
            => new PagedResult<BookShortResponse>()
            {
                Items = pagedResult.Items.ToResponse(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };

        public static BookCreateCommand ToCommand(this BookCreateRequest createRequest)
            => new BookCreateCommand(
                createRequest.Title,
                createRequest.Description,
                createRequest.Price,
                createRequest.StockQuantity,
                createRequest.OwnerIds,
                createRequest.AuthorIds);

        public static BookUpdateCommand ToCommand(this BookUpdateRequest updateRequest)
            => new BookUpdateCommand(
                updateRequest.Id,
                updateRequest.Title,
                updateRequest.Description,
                updateRequest.Price,
                updateRequest.StockQuantity,
                updateRequest.OwnerIds,
                updateRequest.AuthorIds);
    }
}
