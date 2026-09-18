using DigiFikileLms.Application.DTOs.Progress;
using DigiFikileLms.Application.Features.Progress.Commands;
using DigiFikileLms.Application.Features.Progress.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProgressController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // GET STUDENT PROGRESS
    // ================================================================
    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetByStudent(int studentId)
    {
        var query = new GetStudentProgressQuery(studentId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET COURSE PROGRESS
    // ================================================================
    [HttpGet("course/{courseId}")]
    [Authorize(Roles = "SetaAdministrator,Facilitator")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var query = new GetCourseProgressQuery(courseId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // UPDATE PROGRESS - FIXED
    // ================================================================
    [HttpPut]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Update([FromBody] UpdateProgressDto request)
    {
        var command = new UpdateProgressCommand(
            request.StudentId,
            request.CourseId,
            request.Percentage,
            request.Status
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // MARK MODULE COMPLETE
    // ================================================================
    [HttpPost("module/complete")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MarkModuleComplete([FromBody] MarkModuleCompleteDto request)
    {
        var command = new MarkModuleCompleteCommand(request.StudentId, request.ModuleId);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // MARK COURSE COMPLETE
    // ================================================================
    [HttpPost("course/complete")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MarkCourseComplete([FromBody] MarkCourseCompleteDto request)
    {
        var command = new MarkCourseCompleteCommand(request.StudentId, request.CourseId);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

