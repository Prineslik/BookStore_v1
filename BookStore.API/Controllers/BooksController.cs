using AutoMapper;
using BookStore.API.Extensions;
using BookStore.API.Mapping;
using BookStore.API.Requests.Book;
using BookStore.API.Responses.Book;
using BookStore.Application.Authorization;
using BookStore.Application.Contracts.Books.Commands;
using BookStore.Application.Contracts.Books.Queries;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions;
using BookStore.Application.Interfaces.Books;
using BookStore.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore.API.Controllers
{
    [ApiController]
    
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;
        //private readonly IMapper _mapper;

        public BooksController(IBooksService booksService)
        {
            _booksService = booksService;
        }

        //[AllowAnonymous]
        [Authorize(Policy = "books.read")]
        [HttpGet("all/")]
        public async Task<ActionResult<List<BookShortResponse>>> GetAllBooks()
        {
            var booksResult = await _booksService.GetAllBooks();

            if (booksResult.IsFailure)
                return booksResult.Error!.ToErrorResult(HttpContext);

            return Ok(booksResult.Value.ToResponse());
        }

        [HttpGet("get_by_id/{id:guid}")]
        [ProducesResponseType(typeof(BookDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDetailsResponse>> GetBookById(Guid id)
        {
            var bookResult = await _booksService.GetBookById(id);

            if (bookResult.IsFailure)
                return bookResult.Error!.ToErrorResult(HttpContext);

            return bookResult.Value.ToResponse();
        }

        [HttpGet("search/{title}")]
        [ProducesResponseType(typeof(List<BookShortResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<BookShortResponse>>> GetBooksByTitle(string title)
        {
            var booksResult = await _booksService.GetBooksByTitle(title);

            if (booksResult.IsFailure)
                return booksResult.Error!.ToErrorResult(HttpContext);

            return booksResult.Value.ToResponse();
        }

        [HttpGet("filter/")]
        [ProducesResponseType(typeof(PagedResult<BookShortResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<BookShortResponse>>> GetPagedBooksAsync([FromQuery] BookQueryParameters parameters)
        {
            var pagedBooksResult = await _booksService.GetPagedBooks(parameters);

            if (pagedBooksResult.IsFailure)
                return pagedBooksResult.Error!.ToErrorResult(HttpContext);

            var pagedBooksResponse = pagedBooksResult.Value.ToResponse();

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                pagedBooksResponse.TotalCount,
                pagedBooksResponse.PageSize,
                pagedBooksResponse.PageNumber,
                pagedBooksResponse.TotalPages,
                pagedBooksResponse.HasPrevious,
                pagedBooksResponse.HasNext
            }));

            return Ok(pagedBooksResponse);
        }

        //[AllowAnonymous]
        [Authorize(Policy = "books.create")]
        [HttpPost("create/")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateBook([FromBody] BookCreateRequest createBookRequest)
        {
            var createCommand = createBookRequest.ToCommand();

            var bookCreateResult = await _booksService.CreateBook(createCommand);

            if (bookCreateResult.IsFailure)
                return bookCreateResult.Error!.ToErrorResult(HttpContext);

            return Ok(bookCreateResult.Value);
        }

        [HttpPut("update/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> UpdateBook([FromBody] BookUpdateRequest updateBookRequest)
        {
            var updateCommand = new BookUpdateCommand(
                updateBookRequest.Id,
                updateBookRequest.Title,
                updateBookRequest.Description,
                updateBookRequest.Price,
                updateBookRequest.StockQuantity,
                updateBookRequest.OwnerIds,
                updateBookRequest.AuthorIds);

            var bookUpdateResult = await _booksService.UpdateBook(updateCommand);
            
            if (bookUpdateResult.IsFailure)
                return bookUpdateResult.Error!.ToErrorResult(HttpContext);

            return Ok(bookUpdateResult.Value);
        }

        //[HttpPut("change_owners/{id:guid}")]
        //[ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<Guid>> ChangeBookOwners(ChangeBookOwnersRequest changeBookOwnersRequest)
        //{
        //    var bookResult = await _booksService.ChangeBookOwners(
        //        changeBookOwnersRequest.Id, 
        //        changeBookOwnersRequest.OwnersIds);
            
        //    return bookResult;
        //}

        [HttpDelete("delete/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> DeleteBook(Guid id)
        {
            var bookDeleteResult = await _booksService.DeleteBook(id);

            if (bookDeleteResult.IsFailure)
                return bookDeleteResult.Error!.ToErrorResult(HttpContext);

            return Ok(bookDeleteResult.Value);
        }
    }
}
