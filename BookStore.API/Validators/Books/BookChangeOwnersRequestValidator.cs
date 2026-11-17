using BookStore.API.Requests.Book;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.API.Validators.Books;

public class BookChangeOwnersRequestValidator: AbstractValidator<BookChangeOwnersRequest>
{
    public BookChangeOwnersRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull().WithMessage("ID книги обязательно.");

        RuleFor(x => x.OwnersIds)
            .NotEmpty().WithMessage("Список владельцев книги обязателен.");
        //RuleFor(x => x.Title)
        //    .NotEmpty().WithMessage("Название обязательно")
        //    .MaximumLength(250).WithMessage("Название не может быть длиннее 250 символов")
        //    .Matches(@"^[a-zA-Z\s-]+$").WithMessage("Имя может содержать только буквы латинского алфавита, пробелы и дефис");

        //RuleFor(x => x.Description)
        //    .MaximumLength(250).WithMessage("Описание не может быть длиннее 250 символов");

        //RuleFor(x => x.Title)
        //    .NotEmpty().WithMessage("Цена обязательна")
        //    .Matches(@"^(?:0|[1-9]\d*)(?:[\.,]\d{1,2})?$").WithMessage("Имя может содержать только буквы латинского алфавита, пробелы и дефис");
    }
}
