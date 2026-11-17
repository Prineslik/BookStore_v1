using BookStore.API.Responses.User;

namespace BookStore.API.Responses.Book
{
    public record BookDetailsResponse(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        int StockQuantity,
        DateTime CreateDate,
        DateTime? UpdateDate,
        List<OwnerShortResponse> Owners,
        List<AuthorShortResponse> Authors);
}
