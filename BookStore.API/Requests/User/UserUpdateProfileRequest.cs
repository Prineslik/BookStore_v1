namespace BookStore.API.Requests.User
{
    public record UserUpdateProfileRequest
    (
        Guid Id,
        string Name,
        string Email,
        string Password,
        string ProfilePhotoURL,
        List<Guid> RoleIds
    );
}