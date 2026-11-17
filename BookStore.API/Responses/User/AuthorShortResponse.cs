namespace BookStore.API.Responses.User
{
    public record AuthorShortResponse(
        Guid Id,
        string Name,
        string ProfilePhotoURL);
}
