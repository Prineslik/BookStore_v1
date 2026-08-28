using Microsoft.AspNetCore.Authorization;

namespace BookStore.Application.Authorization.Attributes
{
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public string Permission { get; }

        public RequirePermissionAttribute(string permission)
        {
            Permission = permission;
            Policy = $"Permission_{permission}";
        }
    }
}
