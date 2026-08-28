using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles;
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
        public IRolesService _rolesService;
        private readonly IMapper _mapper;

        public RolesController(IRolesService rolesService, IMapper mapper)
        {
            _rolesService = rolesService;
            _mapper = mapper;
        }

        //[Authorize()]
        [HttpGet("all/")]
        public async Task<ActionResult<List<RolesResponse>>> GetAllRoles()
        {
            var allRoles = await _rolesService.GetAllRoles();

            var response = _mapper.Map<List<RolesResponse>>(allRoles);

            return Ok(response);
        }

        [HttpGet("get_by_id/{id:guid}")]
        [ProducesResponseType(typeof(PagedResult<RolesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RolesResponse>> GetRoleById(Guid id)
        {
            var roleById = await _rolesService.GetRoleById(id);

            var roleResponse = _mapper.Map<RolesResponse>(roleById);

            return roleResponse;
        }

        [HttpGet("search/{name}")]
        [ProducesResponseType(typeof(List<RolesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<RolesResponse>>> GetRolesByName(string name)
        {
            var roleByName = await _rolesService.GetRolesByName(name);

            var roleResponse = _mapper.Map<List<RolesResponse>>(roleByName);

            return roleResponse;
        }

        [HttpGet("filter/")]
        [ProducesResponseType(typeof(PagedResult<RolesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<RolesResponse>>> GetPagedRolessAsync([FromQuery] RoleQueryParameters parameters)
        {
            var pagedRoles = await _rolesService.GetPagedRoles(parameters);

            // Добавляем пагинационные метаданные в заголовки
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(new
            {
                pagedRoles.TotalCount,
                pagedRoles.PageSize,
                pagedRoles.PageNumber,
                pagedRoles.TotalPages,
                pagedRoles.HasPrevious,
                pagedRoles.HasNext
            }));

            return Ok(pagedRoles);
        }

        [HttpPost("create/")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateRole([FromBody] RolesRequest rolesRequest)
        {
            var roleEntity = _mapper.Map<RoleEntity>(rolesRequest);

            var roleCreate = await _rolesService.CreateRole(roleEntity);

            return roleCreate;
        }

        [HttpPut("update/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> UpdateRole([FromBody] RolesRequest rolesRequest, Guid id)
        {

            var roleEntity = _mapper.Map<RoleEntity>(rolesRequest with { Id = id });

            var updatedRole = await _rolesService.UpdateRole(roleEntity);

            return updatedRole;
            /*return roleUpdateResult.IsSuccess
                ? Ok(roleUpdateResult.Value)
                : BadRequest(roleUpdateResult.Error);*/
        }

        [HttpPut("add_permissions/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddPermissionsToRole(Guid id, [FromBody] HashSet<Guid?> permissionIds )
        {
            var addPermissions = await _rolesService.AddPermissions(id, permissionIds);

            return addPermissions;
            /*return addPermissionsResult.IsSuccess
                ? Ok(addPermissionsResult.Value)
                : BadRequest(addPermissionsResult.Error);*/
        }

        [HttpDelete("delete/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> DeleteRole(Guid id)
        {
            var roleDeleteResult = await _rolesService.DeleteRole(id);

            return roleDeleteResult;
            /*return roleDeleteResult.IsSuccess
                ? Ok(roleDeleteResult.Value)
                : BadRequest(roleDeleteResult.Error);*/
        }
    }
}
