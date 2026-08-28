using BookStore.Application.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace BookStore.API.Providers
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string Prefix = "Permission_";

        private readonly DefaultAuthorizationPolicyProvider _fallback;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
            => _fallback = new DefaultAuthorizationPolicyProvider(options);

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallback.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallback.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(Prefix))
            {
                var permission = policyName[Prefix.Length..];
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();
                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return _fallback.GetPolicyAsync(policyName);
        }
    }
}
