using BookStore.Application.Contracts.Books;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Validators.Books
{
    public class BooksRequestValidator: AbstractValidator<BooksRequest>
    {
        public BooksRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название обязательно")
                .MaximumLength(250).WithMessage("Название не может быть длиннее 250 символов")
                .Matches(@"^[a-zA-Z\s-]+$").WithMessage("Имя может содержать только буквы латинского алфавита, пробелы и дефис");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Описание не может быть длиннее 250 символов");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Цена обязательна")
                .Matches(@"^(?:0|[1-9]\d*)(?:[\.,]\d{1,2})?$").WithMessage("Имя может содержать только буквы латинского алфавита, пробелы и дефис");
        }
    }
}
