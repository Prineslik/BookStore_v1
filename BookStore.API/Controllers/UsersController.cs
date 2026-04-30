using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Application.Interfaces.Users;
using BookStore.Application.Services;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        public IUsersService _usersService { get; set; }
        private readonly IMapper _mapper;

        public int MyProperty { get; set; }

        public UsersController(IUsersService userService, IMapper mapper)
        {
            _usersService = userService;
            _mapper = mapper;
        }

        [HttpGet("all/")]
        public async Task<ActionResult<List<UsersResponse>>> GetAllBooks()
        {
            var users = await _usersService.GetAllUsers();

            var response = _mapper.Map<List<UsersResponse>>(users);//books.Select(b => new BooksResponse(b.Id, b.Title, b.Description, b.Price));

            return Ok(response);
        }

        [HttpGet("get_by_id/{id:guid}")]
        [ProducesResponseType(typeof(PagedResult<UsersResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsersResponse>> GetUserById(Guid id)
        {
            var user = await _usersService.GetUserById(id);

            if (user == null)
                return NotFound();

            var response = _mapper.Map<UsersResponse>(user);//new UsersResponse(book.Id, book.Title, book.Description, book.Price);

            return Ok(response);
        }

        [HttpPost("create/")]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] UsersRequest usersRequest)
        {
            //var (user, error) = UserEntity.Create(
            //    Guid.NewGuid(),
            //    usersRequest.Name,
            //    usersRequest.Email,
            //    usersRequest.Password,
            //    usersRequest.ProfilePhotoURL);

            //if (!string.IsNullOrEmpty(error))
            //{
            //    return BadRequest(error);
            //}

            await _usersService.CreateUser(user);

            //var problemDetails = new ProblemDetails 
            //{
            //    Status = statusCode,
            //    Title = title,
            //    Detail = detail,
            //    Instance = context.Request.Path,
            //    Extensions = { ["traceId"] = context.TraceIdentifier }
            //};

            return Ok(user.Id);
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            var userId = await _usersService.DeleteUser(id);

            return Ok(userId);
        }

        [HttpGet("search/{title}")]
        public async Task<ActionResult<BooksResponse>> GetUsersByEmail(string email)
        {
            var users = await _usersService.GetUsersByEmail(email);

            if (users == null)
                return NotFound();

            var response = _mapper.Map<List<BooksResponse>>(users);//books.Select(b => new BooksResponse(b.Id, b.Title, b.Description, b.Price)).ToList();

            return Ok(response);
        }

        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateUser(Guid id, [FromBody] UsersRequest usersRequest)
        {
            var (user, error) = UserEntity.Create(
                id,
                usersRequest.Name,
                usersRequest.Email,
                usersRequest.Password,
                usersRequest.ProfilePhotoURL);

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            var userId = await _usersService.UpdateUser(user);

            return Ok(userId);
        }

    }
}
