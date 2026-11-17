namespace BookStore.API.Responses.User
{
    public record UserShortResponse(
        Guid Id,
        string Name,
        string Email,
        string ProfilePhotoURL,
        bool IsAuthor);
}
