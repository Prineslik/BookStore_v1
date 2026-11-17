namespace BookStore.API.Responses.Permission
{
    public record PermissionShortResponse(
        Guid Id,
        string Code,
        string Description);
}
