namespace BookStore.Application.Contracts.Roles
{
    public record RolesRequest(
        Guid Id,
        string Name,
        IReadOnlyCollection<Guid?> PermissionIds
    );
}