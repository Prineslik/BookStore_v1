namespace BookStore.API.Responses.Book
{
    public record BookShortResponse(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        int StockQuantity,
        DateTime CreateDate,
        DateTime? UpdateDate);
}
