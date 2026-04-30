namespace BookStore.Application.Contracts.Users
{
    public record UsersResponse
    (
        Guid Id,
        string Name,
        string Email,
        string PasswordHash,
        string ProfilePhotoURL
    );
}
