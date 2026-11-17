using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionCode { get; private set; }

        public PermissionRequirement(string permissionCode)
        {
            PermissionCode = permissionCode;
        }
    }
}
