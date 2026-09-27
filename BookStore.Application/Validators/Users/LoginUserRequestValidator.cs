using BookStore.Application.Contracts.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Validators.Users
{
    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен FV")
            .EmailAddress().WithMessage("Неверный формат email FV");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен FV");
        }
    }
}
