using BookStore.Application.Authorization.Attributes;
using BookStore.Application.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Authorization.Handlers
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
        {
            //var requiredPermission = requirement.Permission;//context.Resource?.ToString();

            if (context.User.HasClaim("Permission", requirement.Permission))
                context.Succeed(requirement);
            else
                //context.Fail();
                context.Fail(new AuthorizationFailureReason(this,
                $"У пользователя нет разрешения разрешения '{requirement.Permission}'"));

            return Task.CompletedTask;
        }

    }
}
