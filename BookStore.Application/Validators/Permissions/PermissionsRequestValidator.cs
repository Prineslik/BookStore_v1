using BookStore.Application.Contracts.Permissions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Validators.Permissions
{
    public class PermissionsRequestValidator : AbstractValidator<PermissionsRequest>
    {
        public PermissionsRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Код разрешения обязателен")
                .MaximumLength(250).WithMessage("Код не может быть длиннее 250 символов")
                .Matches(@"^[a-zA-Z\s-]+$").WithMessage("Имя может содержать только буквы латинского алфавита, пробелы и дефис");

            RuleFor(x => x.Description)
                //.NotEmpty().WithMessage("Код разрешения обязателен")
                .MaximumLength(250).WithMessage("Код не может быть длиннее 250 символов")
                .Matches(@"^[a-zA-Z\s-]+$").WithMessage("Имя может содержать только буквы латинского алфавита, пробелы и дефис");
        }
    }
}
