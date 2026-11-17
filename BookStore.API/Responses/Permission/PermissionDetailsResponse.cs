using BookStore.API.Responses.Role;

namespace BookStore.API.Responses.Permission
{
    public record PermissionDetailsResponse(
        Guid Id,
        string Code,
        string Description,
        List<RoleShortResponse> RoleIds);
}
