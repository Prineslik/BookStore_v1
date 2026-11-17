namespace BookStore.API.Requests.Permission
{
    public record PermissionCreateRequest(
        string Code,
        string Description);
}
