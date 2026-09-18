using DigiFikileLms.Application.DTOs.SystemAdmin;
using DigiFikileLms.Application.Features.SystemAdmin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/system-admin")]
[Authorize(Roles = "SystemAdministrator")]
public class SystemAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Get system administrator's profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var query = new GetSystemAdminProfileQuery(userId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a SETA Administrator account
    /// </summary>
    [HttpPost("seta-admin")]
    public async Task<IActionResult> CreateSetaAdmin([FromBody] CreateSetaAdminDto request)
    {
        var command = new CreateSetaAdministratorCommand(
            request.Name,
            request.Surname,
            request.Email,
            request.Password,
            request.Phone,
            request.Address,
            request.Permissions,
            GetCurrentUserId()
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Register a SETA Administrator with system generated credentials. The response carries the
    /// username and the temporary password that the account holder must receive (and replace
    /// through POST /api/Auth/password/change after the first login).
    /// </summary>
    [HttpPost("seta-admin/provision")]
    public async Task<IActionResult> ProvisionSetaAdmin([FromBody] ProvisionSetaAdminDto request)
    {
        var command = new ProvisionSetaAdminCommand(
            request.Name,
            request.Surname,
            request.Email,
            request.Phone,
            request.Address,
            request.Permissions,
            GetCurrentUserId()
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get all SETA Administrators
    /// </summary>
    [HttpGet("seta-admins")]
    public async Task<IActionResult> GetAllSetaAdmins([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetAllSetaAdministratorsQuery(page, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get a single SETA Administrator by profile id
    /// </summary>
    [HttpGet("seta-admin/{id:int}")]
    public async Task<IActionResult> GetSetaAdminById(int id)
    {
        var query = new GetSetaAdministratorByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Update a SETA Administrator account/profile
    /// </summary>
    [HttpPut("seta-admin/{id:int}")]
    public async Task<IActionResult> UpdateSetaAdmin(int id, [FromBody] UpdateSetaAdminDto request)
    {
        var command = new UpdateSetaAdminCommand(
            id,
            request.Name,
            request.Surname,
            request.Email,
            request.Phone,
            request.Address,
            request.IsActive,
            GetCurrentUserId());
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Soft-delete/decommission a SETA Administrator account
    /// </summary>
    [HttpDelete("seta-admin/{id:int}")]
    public async Task<IActionResult> DeleteSetaAdmin(int id)
    {
        var result = await _mediator.Send(new DeleteSetaAdminCommand(id, GetCurrentUserId()));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Update SETA Administrator permissions
    /// </summary>
    /**
    [HttpPut("seta-admin/{userId}/permissions")]
    public async Task<IActionResult> UpdatePermissions(int userId, [FromBody] List<SystemAdminPermissionDto> permissions)
    {
        var command = new UpdateSetaAdminPermissionsCommand(userId, permissions);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    **/

    [HttpPut("seta-admin/{userId}/permissions")]        //possible conflict route
    public async Task<IActionResult> UpdateSetaPermissions(int userId, [FromBody] List<SystemAdminPermissionDto> permissions)
    {
        var command = new UpdateSetaAdminPermissionsCommand(userId, permissions);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }


    /// <summary>
    /// Deactivate a SETA Administrator
    /// </summary>
    [HttpPut("seta-admin/{userId}/deactivate")]
    public async Task<IActionResult> DeactivateSetaAdmin(int userId)
    {
        var command = new DeactivateSetaAdminCommand(userId, GetCurrentUserId());
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPut("seta-admin/{userId}/activate")]
    public async Task<IActionResult> ActivateSetaAdmin(int userId)
    {
        var command = new ActivateSetaAdminCommand(userId, GetCurrentUserId());
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestDto request)
    {
        var command = new CreateRoleCommand(request.Name, request.Description);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var query = new GetAllRolesQuery();
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("roles/{roleId}")]
    public async Task<IActionResult> GetRoleById(int roleId)
    {
        var query = new GetRoleByIdQuery(roleId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}