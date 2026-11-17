using BookStore.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Services
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

            return Task.FromResult(policy);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallbackPolicyProvider.GetDefaultPolicyAsync();
        
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    }
}
