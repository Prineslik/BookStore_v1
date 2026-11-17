using AutoMapper;
using BookStore.API.Extensions;
using BookStore.API.Mapping;
using BookStore.API.Requests.Role;
using BookStore.API.Responses.Role;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles.DTO;
using BookStore.Application.Contracts.Roles.Queries;
using BookStore.Application.Interfaces.Roles;
using BookStore.Application.Services;
using BookStore.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using System.Text.Json;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService, IMapper mapper)
        {
            _rolesService = rolesService;
        }

        //[Authorize()]
        [HttpGet("all/")]
        public async Task<ActionResult<List<RoleShortResponse>>> GetAllRoles()
        {
            var rolesResult = await _rolesService.GetAllRoles();

            if (rolesResult.IsFailure)
                return rolesResult.Error!.ToErrorResult(HttpContext);

            return Ok(rolesResult.Value.ToResponse());
        }

        [HttpGet("get_by_id/{id:guid}")]
        [ProducesResponseType(typeof(RoleDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDetailsResponse>> GetRoleById(Guid id)
        {
            var roleByIdResult = await _rolesService.GetRoleById(id);

            if (roleByIdResult.IsFailure)
                return roleByIdResult.Error!.ToErrorResult(HttpContext);
             
            return Ok(roleByIdResult.Value.ToResponse());
        }

        [HttpGet("search/{name}")]
        [ProducesResponseType(typeof(List<RoleShortResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<RoleShortResponse>>> GetRolesByName(string name)
        {
            var roleByNameResult = await _rolesService.GetRolesByName(name);

            if (roleByNameResult.IsFailure)
                return roleByNameResult.Error!.ToErrorResult(HttpContext);

            return Ok(roleByNameResult.Value.ToResponse());
        }

        [HttpGet("filter/")]
        [ProducesResponseType(typeof(PagedResult<RoleShortResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<RoleShortResponse>>> GetPagedRolessAsync([FromQuery] RoleQueryParameters parameters)
        {
            var pagedRolesResult = await _rolesService.GetPagedRoles(parameters);

            if (pagedRolesResult.IsFailure)
                return pagedRolesResult.Error!.ToErrorResult(HttpContext);

            var pagedRolesResponse = pagedRolesResult.Value.ToResponse();

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                pagedRolesResponse.TotalCount,
                pagedRolesResponse.PageSize,
                pagedRolesResponse.PageNumber,
                pagedRolesResponse.TotalPages,
                pagedRolesResponse.HasPrevious,
                pagedRolesResponse.HasNext
            }));

            return Ok(pagedRolesResponse);
        }

        [HttpPost("create/")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateRole([FromBody] RoleCreateRequest roleCreateRequest)
        {
            var createCommand = roleCreateRequest.ToCommand();//RoleEntity.Create(rolesRequest.Name, rolesRequest.PermissionIds, rolesRequest.);

            var roleCreateResult = await _rolesService.CreateRole(createCommand);

            if (roleCreateResult.IsFailure)
                return roleCreateResult.Error!.ToErrorResult(HttpContext);

            return Ok(roleCreateResult.Value);
        }

        [HttpPut("update/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> UpdateRole([FromBody] RoleUpdateRequest rolesRequest, Guid id)
        {
            var updateCommand = rolesRequest.ToCommand();
            var updatedRoleResult = await _rolesService.UpdateRole(updateCommand);

            if (updatedRoleResult.IsFailure)
                return updatedRoleResult.Error!.ToErrorResult(HttpContext);

            return Ok(updatedRoleResult.Value);
        }

        [HttpPut("add_permissions/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddPermissionsToRole(RoleAddPermissionsRequest addPermissionsRequest/*Guid id, [FromBody] List<Guid> permissionIds */)
        {
            var addPermissionsCommand = addPermissionsRequest.ToCommand();

            var addPermissionsResult = await _rolesService.AddPermissions(addPermissionsCommand);

            if (addPermissionsResult.IsFailure)
                return addPermissionsResult.Error!.ToErrorResult(HttpContext);

            return addPermissionsResult.Value;
        }
        
        [HttpDelete("delete/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> DeleteRole(Guid id)
        {
            var roleDeleteResult = await _rolesService.DeleteRole(id);

            if (roleDeleteResult.IsFailure)
                return roleDeleteResult.Error!.ToErrorResult(HttpContext);

            return NoContent();
        }
    }
}
