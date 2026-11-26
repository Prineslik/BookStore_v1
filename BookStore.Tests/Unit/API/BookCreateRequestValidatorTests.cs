using BookStore.API.Requests.Book;
using BookStore.API.Validators.Books;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentValidation.TestHelper;

namespace BookStore.Tests.Unit.API
{
    public class BookCreateRequestValidatorTests
    {
        private readonly BookCreateRequestValidator _validator;

        public static TheoryData<string> GetInvalidTitlesData =>
            new()
            {
                { ""},
                { " "},
                {"\n" },
                {"\t" },
                {"\r" },
                { new string('s', 251)}
            };

        public static TheoryData<string> GetValidTitlesData =>
            new()
            {
                {"asda" },
                {"asdaASDasdga sdag" },
                { new string('s', 250)}
            };

        public BookCreateRequestValidatorTests()
        {
            _validator = new BookCreateRequestValidator();
        }

        [Theory]
        [MemberData(nameof(GetInvalidTitlesData))]
        public void CreateRequest_WhenTitleIsInvalid_ShouldHaveValidationError(string name)
        {
            var request = new BookCreateRequest(
                name,
                "description",
                14.00m,
                1,
                new List<Guid>() { Guid.NewGuid() },
                new List<Guid>() { Guid.NewGuid() });

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(v => v.Title);
        }

        [Theory]
        [MemberData(nameof(GetValidTitlesData))]
        public void CreateRequest_WhenTitleIsValid_ShouldNotHaveValidationError(string name)
        {
            var request = new BookCreateRequest(
                name,
                "description",
                14.00m,
                1,
                new List<Guid>() { Guid.NewGuid() },
                new List<Guid>() { Guid.NewGuid() });

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(v => v.Title);
        }
    }
}
