using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
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
        public IBooksService _booksService;
        private readonly IMapper _mapper;

        public BooksController(IBooksService booksService, IMapper mapper)
        {
            _booksService = booksService;
            _mapper = mapper;
        }

        //[Authorize()]
        [HttpGet("all/")]
        public async Task<ActionResult<List<BooksResponse>>> GetAllBooks()
        {
            var booksResult = await _booksService.GetAllBooks();

            var response = _mapper.Map<List<BooksResponse>>(booksResult);//_mapper.Map<List<BooksResponse>>(booksResult.Value);

            return Ok(response);
        }

        [HttpGet("get_by_id/{id:guid}")]
        [ProducesResponseType(typeof(BooksResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BooksResponse>> GetBookById(Guid id)
        {
            var bookResult = await _booksService.GetBookById(id);

            return _mapper.Map<BooksResponse>(bookResult);
            /*return bookResult.IsSuccess 
                ? Ok(_mapper.Map<BooksResponse>(bookResult.Value))
                : NotFound(bookResult.Error);*/
        }

        [HttpGet("search/{title}")]
        [ProducesResponseType(typeof(List<BooksResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<BooksResponse>>> GetBooksByTitle(string title)
        {
            var booksResult = await _booksService.GetBooksByTitle(title);

            return _mapper.Map<List<BooksResponse>>(booksResult);
            /*return booksResult.IsSuccess
                ? Ok(_mapper.Map<List<BooksResponse>>(booksResult.Value))
                : BadRequest(booksResult.Error);*/
        }

        [HttpGet("filter/")]
        [ProducesResponseType(typeof(PagedResult<BooksResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<BooksResponse>>> GetPagedBooksAsync([FromQuery] BookQueryParameters parameters)
        {
            var pagedBooksResult = await _booksService.GetPagedBooks(parameters);

            /*if (pagedBooksResult.IsFailure)
                return NotFound(pagedBooksResult.Error);*/

            // Добавляем пагинационные метаданные в заголовки
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                pagedBooksResult.TotalCount,
                pagedBooksResult.PageSize,
                pagedBooksResult.PageNumber,
                pagedBooksResult.TotalPages,
                pagedBooksResult.HasPrevious,
                pagedBooksResult.HasNext
            }));

            return Ok(pagedBooksResult);
        }

        [HttpPost("create/")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateBook([FromBody] BooksRequest booksRequest)
        {
            var bookEntity = _mapper.Map<BookEntity>(booksRequest);
            var bookCreateResult = await _booksService.CreateBook(bookEntity);

            return bookCreateResult;
            /*return bookCreateResult.IsSuccess
                ? Ok(bookCreateResult.Value)
                : BadRequest(bookCreateResult.Error);*/
        }

        [HttpPut("update/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> UpdateBook(/*Guid id, */[FromBody] BooksRequest booksRequest)
        {
            var bookEntity = _mapper.Map<BookEntity>(booksRequest);

            var bookUpdateResult = await _booksService.UpdateBook(bookEntity);

            return bookUpdateResult;
            /*return bookUpdateResult.IsSuccess 
                ? Ok(bookUpdateResult.Value)
                : BadRequest(bookUpdateResult.Error);*/
        }

        [HttpDelete("delete/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> DeleteBook(Guid id)
        {
            var bookDeleteResult = await _booksService.DeleteBook(id);

            return bookDeleteResult;
            /*return bookDeleteResult.IsSuccess
                ? Ok(bookDeleteResult.Value)
                : BadRequest(bookDeleteResult.Error);*/
        }
    }
}
