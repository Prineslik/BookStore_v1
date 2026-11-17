using BookStore.API.Requests.Role;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.API.Validators.Roles
{
    public class RoleUpdateRequestValidator : AbstractValidator<RoleUpdateRequest>
    {
        public RoleUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("ID роли обязательно.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название роли обязательно")
                .MaximumLength(250).WithMessage("Название не может быть длиннее 250 символов")
                .Matches(@"^[а-яА-Яa-zA-Z\s-]+$").WithMessage("Название может содержать только буквы, пробелы и дефис");

            RuleFor(x => x.PermissionIds)
                .NotEmpty().WithMessage("Список разрешений роли обязателен.");
        }
    }
}
