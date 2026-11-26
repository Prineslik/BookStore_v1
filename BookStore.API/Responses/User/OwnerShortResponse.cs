namespace BookStore.API.Responses.User
{
    public record OwnerShortResponse(
        Guid Id,
        string Name,
        //string Email,
        string ProfilePhotoURL,
        bool IsAuthor);
}
