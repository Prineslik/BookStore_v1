using BookStore.API.Requests.Book;
using FluentValidation;

namespace BookStore.API.Validators.Books
{
    public class BookUpdateRequestValidator : AbstractValidator<BookUpdateRequest>
    {
        public BookUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("ID книги обязательно.");

            RuleFor(x => x.Title)
               .NotEmpty().WithMessage("Название обязательно.")
               .MaximumLength(250).WithMessage("Название не может быть длиннее 250 символов.")
               .Matches(@"^[a-zA-Z\s-]+$").WithMessage("Имя может содержать только буквы латинского алфавита, пробелы и дефис.");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Описание не может быть длиннее 250 символов.");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Цена обязательна.")
                .GreaterThanOrEqualTo(0).WithMessage("Цена не может быть меньше 0.")
                .LessThanOrEqualTo(Decimal.MaxValue).WithMessage("Превышен лимит цены.");

            RuleFor(x => x.StockQuantity)
                .NotNull().WithMessage("Количество книг в наличии обязательно.")
                .GreaterThanOrEqualTo(0).WithMessage("Количество книг в наличии не может быть меньше 0.")
                .LessThanOrEqualTo(Int32.MaxValue).WithMessage("Превышен лимит количество книг в наличии.");

            RuleFor(x => x.OwnerIds)
                .NotEmpty().WithMessage("Список владельцев книги обязателен.");

            RuleFor(x => x.AuthorIds)
                .NotEmpty().WithMessage("Список авторов книги обязателен.");
        }
    }
}
