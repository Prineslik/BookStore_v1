using BookStore.API.Responses.Permission;
using BookStore.API.Responses.User;
using BookStore.Application.Contracts.Users.DTO;

namespace BookStore.API.Responses.Role
{
    public record RoleDetailsResponse(
        Guid Id,
        string Name,
        List<PermissionShortResponse> PermissionIds,
        List<UserShortResponse> UserIds);
}
