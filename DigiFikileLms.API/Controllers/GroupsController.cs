using DigiFikileLms.Application.DTOs.Group;
using DigiFikileLms.Application.Features.Groups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/groups")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GroupsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Create a new group
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto request)
    {
        var userId = GetCurrentUserId();
        var command = new CreateGroupCommand(
            userId,
            request.setaProgrammeId,
            request.GroupName,
            request.Description
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get all groups for the current administrator
    /// </summary>
    [HttpGet("my-groups")]
    public async Task<IActionResult> GetMyGroups()
    {
        var userId = GetCurrentUserId();
        var query = new GetGroupsByAdministratorQuery(userId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get all groups
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> GetAllGroups([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetAllGroupsQuery(GetCurrentUserId(), page, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get a specific group
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroupById(int id)
    {
        var query = new GetGroupByIdQuery(id, GetCurrentUserId());
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Update a group
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> UpdateGroup(int id, [FromBody] UpdateGroupDto request)
    {
        var command = new UpdateGroupCommand(id, GetCurrentUserId(), request.GroupName, request.Description);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete a group
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> DeleteGroup(int id)
    {
        var command = new DeleteGroupCommand(id, GetCurrentUserId());
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Assign students to a group
    /// </summary>
    [HttpPost("{id}/students")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> AssignStudents(int id, [FromBody] AssignStudentsToGroupDto request)
    {
        var command = new AssignStudentsToGroupCommand(id, GetCurrentUserId(), request.StudentIds);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Remove a student from one of the current SETA Administrator's groups
    /// </summary>
    [HttpDelete("{id}/students/{studentId}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> RemoveStudent(int id, int studentId)
    {
        var command = new RemoveStudentFromGroupCommand(id, GetCurrentUserId(), studentId);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get students in a group
    /// </summary>
    [HttpGet("{id}/students")]
    public async Task<IActionResult> GetGroupStudents(int id)
    {
        var query = new GetGroupStudentsQuery(id, GetCurrentUserId());
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}