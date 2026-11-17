using BookStore.API.Requests.Role;
using FluentValidation;

namespace BookStore.API.Validators.Roles
{
    public class RoleAddPermissionsRequestValidator : AbstractValidator<RoleAddPermissionsRequest>
    {
        public RoleAddPermissionsRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("ID роли обязательно.");

            RuleFor(x => x.PermissionIds)
                .NotEmpty().WithMessage("Список разрешений роли обязателен.");
        }
    }
}
