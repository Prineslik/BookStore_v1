using AutoMapper;
using BookStore.API.Extensions;
using BookStore.API.Mapping;
using BookStore.API.Requests.Permission;
using BookStore.API.Responses.Permission;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions.Commands;
using BookStore.Application.Contracts.Permissions.Queries;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Interfaces.Permissions;
using BookStore.Application.Interfaces.Roles;
using BookStore.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PemissionsController : ControllerBase
    {
        private readonly IPermissionsService _permissionsService;

        public PemissionsController(IPermissionsService permissionsService, IMapper mapper)
        {
            _permissionsService = permissionsService;
        }

        //[Authorize()]
        [HttpGet("all/")]
        public async Task<ActionResult<List<PermissionShortResponse>?>> GetAllPermissions()
        {
            var permissionsResult = await _permissionsService.GetAllPermissions();

            if (permissionsResult.IsFailure)
                return permissionsResult.Error!.ToErrorResult(HttpContext);

            return Ok(permissionsResult.Value.ToResponse());
        }

        [HttpGet("get_by_id/{id:guid}")]
        [ProducesResponseType(typeof(PermissionDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PermissionDetailsResponse>> GetPermissionById(Guid id)
        {
            var permissionResult = await _permissionsService.GetPermissionById(id);

            if (permissionResult.IsFailure)
                return permissionResult.Error!.ToErrorResult(HttpContext);

            return Ok(permissionResult.Value.ToResponse());
        }

        [HttpGet("search/{code}")]
        [ProducesResponseType(typeof(List<PermissionShortResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<PermissionShortResponse>?>> GetPermissionsByCode(string code)
        {
            var permissionResult = await _permissionsService.GetPermissionsByCode(code);

            if (permissionResult.IsFailure)
                return permissionResult.Error!.ToErrorResult(HttpContext);

            return Ok(permissionResult.Value.ToResponse());
        }

        [HttpGet("filter/")]
        [ProducesResponseType(typeof(PagedResult<PermissionShortResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<PermissionShortResponse>>> GetPagedBooksAsync([FromQuery] PermissionQueryParameters parameters)
        {
            var pagedPermissionsResult = await _permissionsService.GetPagedPermissions(parameters);

            if (pagedPermissionsResult.IsFailure)
                return pagedPermissionsResult.Error!.ToErrorResult(HttpContext);

            var pagedPermissionsResponse = pagedPermissionsResult.Value.ToResponse();

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                pagedPermissionsResponse.TotalCount,
                pagedPermissionsResponse.PageSize,
                pagedPermissionsResponse.PageNumber,
                pagedPermissionsResponse.TotalPages,
                pagedPermissionsResponse.HasPrevious,
                pagedPermissionsResponse.HasNext
            }));

            return Ok(pagedPermissionsResponse);
        }

        [HttpPost("create/")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreatePermission([FromBody] PermissionCreateRequest permissionsCreateRequest)
        {
            var createCommand = permissionsCreateRequest.ToCommand();

            var createPermissionResult = await _permissionsService.CreatePermission(createCommand);

            if (createPermissionResult.IsFailure)
                return createPermissionResult.Error!.ToErrorResult(HttpContext);

            return Ok(createPermissionResult.Value);
        }

        [HttpPut("update/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> UpdatePermission(PermissionUpdateRequest updateRequest)
        {
            var updateCommand = updateRequest.ToCommand();

            var updatePermissionResult = await _permissionsService.UpdatePermission(updateCommand);

            if (updatePermissionResult.IsFailure)
                return updatePermissionResult.Error!.ToErrorResult(HttpContext);

            return Ok(updatePermissionResult.Value);
        }

        [HttpDelete("delete/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> DeletePermission(Guid id)
        {
            var deletePermissionResult = await _permissionsService.DeletePermission(id);

            if (deletePermissionResult.IsFailure)
                return deletePermissionResult.Error!.ToErrorResult(HttpContext);

            return Ok(deletePermissionResult.Value);
        }
    }
}
