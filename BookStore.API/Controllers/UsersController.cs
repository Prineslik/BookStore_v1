using AutoMapper;
using BookStore.Application.Authorization.Attributes;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Application.Interfaces.Users;
using BookStore.Application.Services;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
        public async Task<ActionResult<List<UsersResponse?>>> GetAllUsers()
        {
            var resultUsers = await _usersService.GetAllUsers();

            //if (resultUsers.IsFailure)
            //    return BadRequest(resultUsers.Error);

            var response = _mapper.Map<List<UsersResponse?>>(resultUsers);//books.Select(b => new BooksResponse(b.Id, b.Title, b.Description, b.Price));

            return Ok(response);
        }

        [HttpGet("get_by_id/{id:guid}")]
        [ProducesResponseType(typeof(UsersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsersResponse>> GetUserById(Guid id)
        {
            var resultUser = await _usersService.GetUserById(id);

            return _mapper.Map<UsersResponse>(resultUser);
        }

        [HttpGet("filter/")]
        [ProducesResponseType(typeof(PagedResult<UsersResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<BooksResponse?>>> GetPagedUsersAsync([FromQuery] UserQueryParameters parameters)
        {
            var pagedUsers = await _usersService.GetPagedUsersAsync(parameters);

            // Добавляем пагинационные метаданные в заголовки
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                pagedUsers.TotalCount,
                pagedUsers.PageSize,
                pagedUsers.PageNumber,
                pagedUsers.TotalPages,
                pagedUsers.HasPrevious,
                pagedUsers.HasNext
            }));

            return Ok(pagedUsers);
        }

        [HttpPost("create/")]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] UsersRequest usersRequest)
        {
            var userEntity = _mapper.Map<UserEntity>(usersRequest);

            var newUser = await _usersService.CreateUser(userEntity);

            return newUser;
            //return resultCreateUser.IsSuccess ? Ok(resultCreateUser.Value) : BadRequest(resultCreateUser.Error);
        }

        [HttpGet("login/")]
        public async Task<ActionResult> LoginUser([FromBody] LoginUserRequest userRequest)
        {
            var token = await _usersService.LoginUser(userRequest.Email, userRequest.Password);

            /*if(tokenResult.IsFailure)
                return BadRequest(tokenResult.Error);*/

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(7)
            };

            Response.Cookies.Append("RefreshToken", token, cookieOptions);


            return Ok(token);
        }

        //[Authorize(Roles ="Admin")]
        [RequirePermission("Permission_users.delete")]
        [HttpDelete("delete/{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            /*var resultDeleteUser = */
            await _usersService.DeleteUser(id);

            return NoContent();
            //return resultDeleteUser.IsSuccess ? Ok(resultDeleteUser.Value) : BadRequest(resultDeleteUser.Error);
        }

        [HttpGet("search/{email}")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<ActionResult<UsersResponse?>> GetUserByEmail(string email)
        {
            var user = await _usersService.GetUsersByEmail(email);

            return Ok(_mapper.Map<UsersResponse?>(user));
            /*return resultUser.IsSuccess 
                ? Ok(_mapper.Map<UsersResponse>(resultUser.Value)) 
                : NotFound(resultUser.Error);*/
        }

        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateUser(Guid id, [FromBody] UsersRequest userRequest)
        {
            var userEntity = _mapper.Map<UserEntity>(userRequest with { Id = id });

            var resultUpdateUser = await _usersService.UpdateUser(userEntity);

            return Ok(resultUpdateUser);
            //return resultUpdateUser.IsSuccess ? Ok(resultUpdateUser.Value) : BadRequest(resultUpdateUser.Error);
        }
    }
}
