using DigiFikileLms.Application.DTOs.Students;
using DigiFikileLms.Application.Features.Students;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // GET STUDENT PROFILE
    // ================================================================
    [HttpGet("profile/{userId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetProfile(int userId)
    {
        var query = new GetStudentProfileQuery(userId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // UPDATE STUDENT PROFILE
    // ================================================================
    [HttpPut("profile/{userId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] StudentUpdateProfileDto request)
    {
        var command = new UpdateStudentProfileCommand(userId, request.Phone, request.Address);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // GET STUDENT ENROLLMENTS
    // ================================================================
    [HttpGet("{studentId}/enrollments")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetEnrollments(int studentId)
    {
        var query = new GetStudentEnrollmentsQuery(studentId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET STUDENT PROGRESS
    // ================================================================
    [HttpGet("{studentId}/progress")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetProgress(int studentId)
    {
        var query = new GetStudentProgressQuery(studentId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET STUDENT CERTIFICATES
    // ================================================================
    [HttpGet("{studentId}/certificates")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetCertificates(int studentId)
    {
        var query = new GetStudentCertificatesQuery(studentId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET ALL STUDENTS (Admin)
    // ================================================================
    [HttpGet]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetAllStudentsQuery(page, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // GET STUDENT BY ID (Admin)
    // ================================================================
    [HttpGet("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetStudentByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // CREATE STUDENT (Admin)
    // ================================================================
    [HttpPost]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequestDto request)
    {
        var command = new CreateStudentCommand(
            request.Name,
            request.Surname,
            request.Email,
            request.Password,
            request.Phone,
            request.Address,
            request.StudentNumber
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // UPDATE STUDENT (Admin)
    // ================================================================
    [HttpPut("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequestDto request)
    {
        var command = new UpdateStudentCommand(
            id,
            request.Name,
            request.Surname,
            request.Email,
            request.Phone,
            request.Address,
            request.IsActive
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // DELETE STUDENT (Admin)
    // ================================================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteStudentCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}

