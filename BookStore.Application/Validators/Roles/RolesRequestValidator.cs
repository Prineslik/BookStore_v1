using BookStore.Application.Contracts.Roles;
using BookStore.Application.Contracts.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Validators.Roles
{
    public class RolesRequestValidator : AbstractValidator<RolesRequest>
    {
        public RolesRequestValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название роли обязательно")
            .MaximumLength(250).WithMessage("Название не может быть длиннее 250 символов")
            .Matches(@"^[а-яА-Яa-zA-Z\s-]+$").WithMessage("Название может содержать только буквы, пробелы и дефис");
        }
    }
}
