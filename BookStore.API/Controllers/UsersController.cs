using AutoMapper;
using BookStore.API.Extensions;
using BookStore.API.Mapping;
using BookStore.API.Requests.User;
using BookStore.API.Responses.User;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users.Commands;
using BookStore.Application.Contracts.Users.DTO;
using BookStore.Application.Contracts.Users.Queries;
using BookStore.Application.Interfaces.Users;
using BookStore.Application.Services;
using BookStore.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService userService, IMapper mapper)
        {
            _usersService = userService;
        }

        [HttpGet("all/")]
        public async Task<ActionResult<List<UserShortResponse>>> GetAllUsers()
        {
            var usersResult = await _usersService.GetAllUsers();

            if (usersResult.IsFailure)
                return usersResult.Error.ToErrorResult(HttpContext);

            return Ok(usersResult.Value.ToResponse());
        }

        [HttpGet("get_by_id/{id:guid}")]
        [ProducesResponseType(typeof(UsersDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsersDetailsResponse?>> GetUserById(Guid id)
        {
            var usersResult = await _usersService.GetUserById(id);

            if (usersResult.IsFailure)
                return usersResult.Error.ToErrorResult(HttpContext);

            return Ok(usersResult.Value.ToResponse());
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(UsersDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsersDetailsResponse?>> GetMyProfile()
        {
            string? idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(idClaim, out var userId))
                return Error.Validation($"User id {idClaim} is incorrect").ToErrorResult(HttpContext);

            var usersResult = await _usersService.GetUserById(userId);

            if (usersResult.IsFailure)
                return usersResult.Error.ToErrorResult(HttpContext);

            return Ok(usersResult.Value.ToResponse());
        }

        [HttpGet("filter/")]
        [ProducesResponseType(typeof(PagedResult<UsersDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<UserShortResponse>>> GetPagedUsersAsync([FromQuery] UserQueryParameters parameters)
        {
            var pagedUsersResult = await _usersService.GetPagedUsersAsync(parameters);

            if (pagedUsersResult.IsFailure)
                return pagedUsersResult.Error.ToErrorResult(HttpContext);

            var pagedUsersResponse = pagedUsersResult.Value.ToResponse();

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                pagedUsersResponse.TotalCount,
                pagedUsersResponse.PageSize,
                pagedUsersResponse.PageNumber,
                pagedUsersResponse.TotalPages,
                pagedUsersResponse.HasPrevious,
                pagedUsersResponse.HasNext
            }));

            return Ok(pagedUsersResponse);
        }

        [AllowAnonymous]
        [HttpPost("register/")]
        public async Task<ActionResult<Guid>> RegisterUser(string name, string email, string password/*[FromBody] UsersRequest usersRequest*/)
        {
            var registerCommand = new RegisterUserCommand(name, email, password);

            var createResult = await _usersService.CreateUser(registerCommand);

            if (createResult.IsFailure)
                return createResult.Error!.ToErrorResult(HttpContext);

            return Ok(createResult.Value);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> LoginUser([FromBody] LoginUserRequest userRequest)
        {
            var tokenResult = await _usersService.LoginUser(userRequest.Email, userRequest.Password);

            if (tokenResult.IsFailure)
                return tokenResult.Error.ToErrorResult(HttpContext);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(7)
            };

            Response.Cookies.Append("RefreshToken", tokenResult.Value, cookieOptions);

            return Ok(tokenResult.Value);
        }

        //[Authorize(Roles ="Admin")]
        //[RequirePermission("Permission_users.delete")]
        [HttpDelete("delete/{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            var deleteResult = await _usersService.DeleteUser(id);

            if (deleteResult.IsFailure)
                return deleteResult.Error.ToErrorResult(HttpContext);

            return NoContent();
        }

        //[HttpGet("search/{email}")]
        ////[Authorize(Policy = "UserOrAdmin")]
        //public async Task<ActionResult<UsersDetailsResponse?>> GetUserByEmail(string email)
        //{
        //    var userResult = await _usersService.GetUserByEmail(email);

        //    if (userResult.IsFailure)
        //        return BadRequest(userResult.Error);

        //    return Ok(userResult.Value.ToResponse());
        //}

        [HttpPut("update/profile/me")]
        public async Task<ActionResult<Guid>> UpdateOwnProfile([FromBody] UserUpdateProfileRequest userRequest)
        {
            //Guid userId;
            string idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(idClaim, out var userId))
                return Error.Validation($"User id {idClaim} is incorrect").ToErrorResult(HttpContext);

            var updateCommand = new UpdateUserProfileCommand(
                userId,
                userRequest.Name,
                userRequest.Email,
                userRequest.ProfilePhotoURL);

            var resultUpdateUserProfile = await _usersService.UpdateUserProfile(updateCommand);

            if(resultUpdateUserProfile.IsFailure)
                return resultUpdateUserProfile.Error!.ToErrorResult(HttpContext);

            return Ok(resultUpdateUserProfile);
        }

        [Authorize(Policy = "Users.UpdateOtherProfiles")]
        [HttpPut("update/profile/{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateUserProfile([FromQuery] Guid id, [FromBody] UserUpdateProfileRequest userRequest)
        {
            var updateCommand = new UpdateUserProfileCommand(
                id,
                userRequest.Name,
                userRequest.Email,
                userRequest.ProfilePhotoURL);

            var resultUpdateUserProfile = await _usersService.UpdateUserProfile(updateCommand);

            if(resultUpdateUserProfile.IsFailure)
                return resultUpdateUserProfile.Error!.ToErrorResult(HttpContext);

            return Ok(resultUpdateUserProfile);
        }
    }
}
