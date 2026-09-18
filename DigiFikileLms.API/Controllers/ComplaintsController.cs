using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Complaint;
using DigiFikileLms.Application.Features.Complaints.Commands;
using DigiFikileLms.Application.Features.Complaints.Queries;
using DigiFikileLms.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigiFikileLms.API.Controllers;

/// <summary>
/// TYPE: ComplaintsController
/// PURPOSE: HTTP surface for the Complaint feature. Anyone authenticated may lay a complaint and
///          read their own; only SystemAdministrators see and work through every complaint.
/// LMS ROLE: Supports the Complaint area while respecting the API layer boundary.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ComplaintsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ComplaintsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // LAY A COMPLAINT - any authenticated account
    // ================================================================
    /// <summary>
    /// Lays a complaint. The complainant is always the authenticated caller; every complaint
    /// starts in the Open state.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> LayComplaint([FromBody] LayComplaintRequestDto request)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized(BaseResponse<ComplaintDto>.Failure("Authenticated user could not be resolved"));

        var command = new LayComplaintCommand(userId, request.Title, request.Description, request.Category);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // ALL COMPLAINTS - System Administrator only
    // ================================================================
    /// <summary>
    /// Lists every complaint in the system, newest first. The optional status filter accepts one
    /// of: Open, InReview, Resolved, Dismissed.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = nameof(UserRole.SystemAdministrator))]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var result = await _mediator.Send(new GetAllComplaintsQuery(status));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // ONE COMPLAINT - System Administrator only
    // ================================================================
    /// <summary>Fetches one complaint by id.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = nameof(UserRole.SystemAdministrator))]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetComplaintByIdQuery(id));
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // MY COMPLAINTS - the authenticated caller's own complaints
    // ================================================================
    /// <summary>Lists the complaints the authenticated caller laid, newest first.</summary>
    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> GetMine()
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized(BaseResponse<List<ComplaintDto>>.Failure("Authenticated user could not be resolved"));

        var result = await _mediator.Send(new GetMyComplaintsQuery(userId));
        return Ok(result);
    }

    // ================================================================
    // UPDATE STATUS - System Administrator only
    // ================================================================
    /// <summary>
    /// Moves a complaint to a new state. Status accepts one of: Open, InReview, Resolved,
    /// Dismissed; resolution notes are stored when provided.
    /// </summary>
    [HttpPut("{id:int}/status")]
    [Authorize(Policy = nameof(UserRole.SystemAdministrator))]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateComplaintStatusRequestDto request)
    {
        // Bad input is a 400; a complaint that does not exist is a 404. Model binding has already
        // converted the JSON status name to the enum and rejected an unknown name with a 400, so
        // this guard only has to catch an out-of-range numeric value such as {"status": 99}.
        if (!Enum.IsDefined<ComplaintStatus>(request.Status))
        {
            return BadRequest(BaseResponse<ComplaintDto>.Failure(
                "Status must be one of: Open, InReview, Resolved, Dismissed"));
        }

        var command = new UpdateComplaintStatusCommand(id, request.Status, request.ResolutionNotes);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}