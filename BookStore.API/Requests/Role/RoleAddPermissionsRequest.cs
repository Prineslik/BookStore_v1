namespace BookStore.API.Requests.Role
{
    public record RoleAddPermissionsRequest(
        Guid Id,
        IReadOnlyCollection<Guid> PermissionIds);
}
