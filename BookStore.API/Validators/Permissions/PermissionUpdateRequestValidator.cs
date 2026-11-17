using BookStore.API.Requests.Permission;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.API.Validators.Permissions
{
    public class PermissionUpdateRequestValidator : AbstractValidator<PermissionUpdateRequest>
    {
        public PermissionUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("ID разрешения обязательно.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Код разрешения обязателен")
                .MaximumLength(250).WithMessage("Код не может быть длиннее 250 символов")
                .Matches(@"^[a-zA-Z\s-]+$").WithMessage("Имя может содержать только буквы латинского алфавита, пробелы и дефис");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Описание не может быть длиннее 250 символов");
        }
    }
}
