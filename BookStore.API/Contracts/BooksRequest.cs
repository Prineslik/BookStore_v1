namespace BookStore.API.Contracts
{
    public record BooksRequest(
        Guid Id,
        string Title,
        string Description,
        decimal Price
    );
}
