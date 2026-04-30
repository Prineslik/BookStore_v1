namespace BookStore.Application.Contracts.Books
{
    public record BooksResponse(
        Guid Id,
        string Title,
        string Description,
        decimal Price
    );
}