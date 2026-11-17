namespace BookStore.API.Requests.Role
{
    public record RoleCreateRequest(
        string Name,
        IReadOnlyCollection<Guid> PermissionIds);
}
