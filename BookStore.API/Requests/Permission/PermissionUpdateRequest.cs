namespace BookStore.API.Requests.Permission
{
    public record PermissionUpdateRequest(
        Guid Id,
        string Code,
        string Description);
}
