namespace BookStore.Application.Contracts.Books
{
    public record BooksRequest(
        Guid Id,
        string Title,
        string Description,
        decimal Price
    );
}
