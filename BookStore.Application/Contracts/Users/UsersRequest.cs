namespace BookStore.Application.Contracts.Users
{
    public record UsersRequest
    (
        Guid Id,
        string Name,
        string Email,
        string Password,
        string ProfilePhotoURL,
        List<Guid?> RoleIds
    );
}