using BookStore.Application.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BookStore.Application.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUsersReadRepository _usersReadRepository;

        public PermissionHandler(IUsersReadRepository usersReadRepository)
        {
            _usersReadRepository = usersReadRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                context.Fail();
                return;
            }

            if (await _usersReadRepository.HasPermission(userId, requirement.PermissionCode))
                context.Succeed(requirement);
        }
    }
}
