namespace BookStore.Application.Contracts.Users
{
    public record UsersRequest
    (
        string Name,
        string Email,
        string Password,
        string ProfilePhotoURL
    );
}
