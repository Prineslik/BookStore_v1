using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions;
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
        public IPermissionsService _permissionsService;
        private readonly IMapper _mapper;

        public PemissionsController(IPermissionsService permissionsService, IMapper mapper)
        {
            _permissionsService = permissionsService;
            _mapper = mapper;
        }

        //[Authorize()]
        [HttpGet("all/")]
        public async Task<ActionResult<List<PermissionsResponse>?>> GetAllPermissions()
        {
            var allPermissionEntities = await _permissionsService.GetAllPermissions();

            var permissionResponse = _mapper.Map<List<PermissionsResponse>?>(allPermissionEntities);

            return Ok(permissionResponse);
        }

        [HttpGet("get_by_id/{id:guid}")]
        [ProducesResponseType(typeof(PagedResult<PermissionsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PermissionsResponse>> GetPermissionById(Guid id)
        {
            var permissionEntity = await _permissionsService.GetPermissionById(id);

            var permissionResponse = _mapper.Map<PermissionsResponse>(permissionEntity);

            return Ok(permissionResponse);
        }

        [HttpGet("search/{code}")]
        [ProducesResponseType(typeof(List<PermissionsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<RolesResponse>?>> GetPermissionsByCode(string code)
        {
            var permissionEntities = await _permissionsService.GetPermissionsByCode(code);

            var permissionResponse = _mapper.Map<List<PermissionsResponse>?>(permissionEntities);

            return Ok(permissionResponse);
        }

        [HttpGet("filter/")]
        [ProducesResponseType(typeof(PagedResult<PermissionsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<PermissionsResponse>>> GetPagedBooksAsync([FromQuery] PermissionQueryParameters parameters)
        {
            var pagedPermissionEntities = await _permissionsService.GetPagedPermissions(parameters);

            var pagedPermissionsResponse = new PagedResult<PermissionsResponse>
            {
                Items = _mapper.Map<List<PermissionsResponse>?>(pagedPermissionEntities.Items),
                TotalCount = pagedPermissionEntities.TotalCount,
                PageNumber = pagedPermissionEntities.PageNumber,
                PageSize = pagedPermissionEntities.PageSize
            };

            // Добавляем пагинационные метаданные в заголовки
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
        public async Task<ActionResult<Guid>> CreatePermission([FromBody] PermissionsRequest permissionsRequest)//убрать вводимый ИД
        {
            var permissionEntity = _mapper.Map<PermissionEntity>(permissionsRequest);
            var createdPermissionId = await _permissionsService.CreatePermission(permissionEntity);

            return createdPermissionId;
        }

        [HttpPut("update/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> UpdatePermission(Guid id, [FromBody] PermissionsRequest permissionsRequest)
        {
            var updateRequest = permissionsRequest with { Id = id };
            var permissionEntity = _mapper.Map<PermissionEntity>(permissionsRequest);

            var updatedPermissionId = await _permissionsService.UpdatePermission(permissionEntity);

            return updatedPermissionId;
        }

        [HttpDelete("delete/{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> DeletePermission(Guid id)
        {
            var permissionDeleteId = await _permissionsService.DeletePermission(id);

            return permissionDeleteId;
        }
    }
}
