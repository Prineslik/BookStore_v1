using BookStore.API.Requests.Permission;
using FluentValidation;

namespace BookStore.API.Validators.Permissions
{
    public class PermissionCreateRequestValidator : AbstractValidator<PermissionCreateRequest>
    {
        public PermissionCreateRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Код разрешения обязателен")
                .MaximumLength(250).WithMessage("Код не может быть длиннее 250 символов")
                .Matches(@"\b\w+\.\w+\b").WithMessage("Код может содержать только буквы латинского алфавита и точку");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Описание не может быть длиннее 250 символов");
        }
    }
}
