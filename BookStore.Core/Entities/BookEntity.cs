using BookStore.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Core.Entities
{
    public class BookEntity
    {
        public const int MAX_TITLE_LENGTH = 250;
        public const int MAX_DESCRIPTION_LENGTH = 1000;

        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        private readonly List<Guid> _ownersIds = new();
        public IReadOnlyCollection<Guid> OwnerIds => _ownersIds;

        private readonly List<Guid> _authorIds = new();
        public IReadOnlyCollection<Guid> AuthorIds => _authorIds;

        private BookEntity()
        {
        }

        private BookEntity(
            Guid id,
            string title,
            string description,
            decimal price,
            int stockQuantity,
            DateTime createDate,
            DateTime? updateDate,
            List<Guid> ownersIds,
            List<Guid> authorIds)
        {
            Id = id;
            Title = title;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            CreateDate = createDate;
            UpdateDate = updateDate;
            _ownersIds = new List<Guid>(ownersIds);
            _authorIds = new List<Guid>(authorIds);
        }

        public static BookEntity Create( 
            string title, 
            string description, 
            decimal price, 
            int stockQuantity,
            List<Guid> ownerIds, 
            List<Guid> authorIds)
        {
            ValidateTitle(title);
            ValidateDescription(description);
            ValidatePrice(price);
            ValidateStockQuantity(stockQuantity);
            ValidateAuthorIds(authorIds);
            ValidateOwnerIds(ownerIds);

            var book = new BookEntity(
                Guid.NewGuid(),
                title,
                description,
                price,
                stockQuantity,
                DateTime.UtcNow,
                null,
                ownerIds,
                authorIds);

            return book;
        }

        public static BookEntity Reconstitute(
            Guid id, 
            string title, 
            string description, 
            decimal price, 
            int stockQuantity,
            DateTime createDate,
            DateTime? updateDate,
            List<Guid> ownerIds, 
            List<Guid> authorIds)
        {
            return new BookEntity(
                id,
                title,
                description,
                price,
                stockQuantity,
                createDate, 
                updateDate,
                ownerIds,
                authorIds);
        }

        public void Rename(string newTitle)
        {
            newTitle = newTitle.Trim();

            ValidateTitle(newTitle);

            Title = newTitle;
            UpdateDate = DateTime.UtcNow;
        }

        public void ChangeDescription(string newDescription)
        {
            newDescription = newDescription.Trim();

            ValidateDescription(newDescription);

            Description = newDescription;
            UpdateDate = DateTime.UtcNow;
        }

        public void ChangePrice(decimal newPrice)
        {
            ValidatePrice(newPrice);

            Price = newPrice;
            UpdateDate = DateTime.UtcNow;
        }

        public void ChangeStockQuantity(int newStockQuantity)
        {
            ValidateStockQuantity(newStockQuantity);
            
            StockQuantity = newStockQuantity;
            UpdateDate = DateTime.UtcNow;
        }

        public void ChangeOwners(List<Guid> ownerIds)
        {
            ValidateOwnerIds(ownerIds);

            _ownersIds.Clear();
            _ownersIds.AddRange(ownerIds);
            UpdateDate = DateTime.UtcNow;
        }

        public void ChangeAuthors(List<Guid> authorIds)
        {
            ValidateAuthorIds(authorIds);

            _authorIds.Clear();
            _authorIds.AddRange(authorIds);
            UpdateDate = DateTime.UtcNow;
        }

        private static void ValidateTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new DomainException($"Title can not be empty");

            if (title.Length > MAX_TITLE_LENGTH)
                throw new DomainException($"Title can not be longer than {MAX_TITLE_LENGTH}");
        }

        private static void ValidateDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new DomainException($"Description can not be empty");

            if (string.IsNullOrEmpty(description) || description.Length > MAX_DESCRIPTION_LENGTH)
                throw new DomainException($"Description can not be empty or longer than {MAX_DESCRIPTION_LENGTH}");
        }

        private static void ValidatePrice(decimal price)
        {
            if(price < 0)
                throw new DomainException("Price cannot be less than 0");

            if (price.Scale > 2)
                throw new DomainException("Price cannot have more than 2 digits after the decimal point");
        }

        private static void ValidateStockQuantity(int quantity)
        {
            if (quantity < 0)
                throw new DomainException("Stock quantity cannot be less than 0");
        }

        private static void ValidateAuthorIds(List<Guid> authorIds)
        {
            if (authorIds.Count == 0)
                throw new DomainException("List of authors cannot be empty");
        }

        private static void ValidateOwnerIds(List<Guid> ownerIds)
        {
            if (ownerIds.Count == 0)
                throw new DomainException("List of owners cannot be empty");
        }
    }
}
