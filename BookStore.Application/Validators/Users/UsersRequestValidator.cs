using BookStore.Application.Contracts.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Validators.Users
{
    public class UsersRequestValidator : AbstractValidator<UsersRequest>
    {
        public UsersRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя пользователя обязательно")
                .MaximumLength(250).WithMessage("Имя не может быть длиннее 250 символов")
                .Matches(@"^[а-яА-Яa-zA-Z\s-]+$").WithMessage("Имя может содержать только буквы, пробелы и дефис");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Неверный формат email")
                .MaximumLength(250).WithMessage("Email не может быть длиннее 250 символов");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Пароль должен быть не короче 6 символов")
                .MaximumLength(100).WithMessage("Пароль не может быть длиннее 100 символов")
                .Matches(@"[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
                .Matches(@"[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
                .Matches(@"[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру")
                .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("Пароль должен содержать хотя бы один специальный символ");

            RuleFor(x => x.ProfilePhotoURL)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .When(x => !string.IsNullOrEmpty(x.ProfilePhotoURL))
                .WithMessage("Некорректный URL фотографии");

            RuleFor(x => x.RoleIds)
                .NotNull().WithMessage("Список ролей не может быть null")
                .Must(ids => ids.All(id => id != Guid.Empty)).WithMessage("Все ID прав должны быть валидными");
        }
    }
}
