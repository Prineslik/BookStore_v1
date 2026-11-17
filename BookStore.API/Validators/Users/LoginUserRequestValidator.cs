using BookStore.API.Requests.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.API.Validators.Users
{
    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Неверный формат email.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен.");
        }
    }
}
