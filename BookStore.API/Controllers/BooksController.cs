using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Interfaces.Books;
using BookStore.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        public IBooksService _booksService;
        private readonly IMapper _mapper;

        public BooksController(IBooksService booksService, IMapper mapper)
        {
            _booksService = booksService;
            _mapper = mapper;
        }

        [HttpGet("all/")]
        public async Task<ActionResult<List<BooksResponse>>> GetAllBooks()
        {
            var books = await _booksService.GetAllBooks();

            var response = books.Select(b => new BooksResponse(b.Id, b.Title, b.Description, b.Price));

            return Ok(response);
        }

        [HttpGet("get_by_id/{id:guid}")]
        [ProducesResponseType(typeof(PagedResult<BooksResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BooksResponse>> GetBookById(Guid id)
        {
            var book = await _booksService.GetBookById(id);

            if (book == null)
                return NotFound();

            var response = _mapper.Map<BooksResponse>(book);//new BooksResponse(book.Id, book.Title, book.Description, book.Price);

            return Ok(response);
        }

        [HttpGet("search/{title}")]
        public async Task<ActionResult<BooksResponse>> GetBooksByTitle(string title)
        {
            var books = await _booksService.GetBooksByTitle(title);

            var response = _mapper.Map<List<BooksResponse>>(books);//books.Select(b => new BooksResponse(b.Id, b.Title, b.Description, b.Price)).ToList();

            return Ok(response);
        }

        [HttpGet("filter/")]
        //[ProducesResponseType(typeof(PagedResult<BooksResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PagedResult<BooksResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<BooksResponse>>> GetPagedBooksAsync([FromQuery] BookQueryParameters parameters)
        {
            var result = await _booksService.GetPagedBookAsync(parameters);

            if (result == null)
                return NotFound();

            // Добавляем пагинационные метаданные в заголовки
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                result.TotalCount,
                result.PageSize,
                result.PageNumber,
                result.TotalPages,
                result.HasPrevious,
                result.HasNext
            }));

            return Ok(result);
        }

        [HttpPost("create/")]
        public async Task<ActionResult<Guid>> CreateBook([FromBody] BooksRequest booksRequest)
        {
            var (book, error) = BookEntity.Create(
                Guid.NewGuid(),
                booksRequest.Title,
                booksRequest.Description,
                booksRequest.Price);

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            await _booksService.CreateBook(book);

            return Ok(book.Id);
        }

        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateBook(Guid id, [FromBody] BooksRequest booksRequest)
        {
            var (book, error) = BookEntity.Create(
                id/*Guid.NewGuid()*/,
                booksRequest.Title,
                booksRequest.Description,
                booksRequest.Price);

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            var bookId = await _booksService.UpdateBook(book);

            return Ok(bookId);
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteBook(Guid id)
        {
            var bookId = await _booksService.DeleteBook(id);

            return Ok(bookId);
        }
    }
}
