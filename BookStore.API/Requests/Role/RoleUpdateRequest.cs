namespace BookStore.API.Requests.Role
{
    public record RoleUpdateRequest(
        Guid Id,
        string Name,
        IReadOnlyCollection<Guid> PermissionIds);
}
