using DigiFikileLms.Application.DTOs.Enrollment;  // ✅ ADD THIS
using DigiFikileLms.Application.Features.Enrollments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // ENROLL STUDENT IN COURSE
    // ================================================================
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Enroll([FromBody] EnrollStudentDto request)
    {
        var command = new EnrollStudentCommand(request.StudentId, request.CourseId);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // GET ENROLLMENTS BY STUDENT
    // ================================================================
    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetByStudent(int studentId)
    {
        var query = new GetStudentEnrollmentsQuery(studentId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET ENROLLMENTS BY COURSE
    // ================================================================
    [HttpGet("course/{courseId}")]
    [Authorize(Roles = "SetaAdministrator,Facilitator")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var query = new GetCourseEnrollmentsQuery(courseId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // WITHDRAW ENROLLMENT
    // ================================================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Withdraw(int id)
    {
        var command = new WithdrawEnrollmentCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // COMPLETE ENROLLMENT
    // ================================================================
    [HttpPost("{id}/complete")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Complete(int id)
    {
        var command = new CompleteEnrollmentCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

