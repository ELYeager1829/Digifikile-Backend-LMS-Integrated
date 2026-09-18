using DigiFikileLms.Application.DTOs.Courses;
using DigiFikileLms.Application.Features.Courses.Commands;
using DigiFikileLms.Application.Features.Courses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // GET ALL COURSES
    // ================================================================
    [HttpGet]
    [Authorize(Roles = "SetaAdministrator,Facilitator")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetAllCoursesQuery(page, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // GET PUBLISHED COURSES
    // ================================================================
    [HttpGet("published")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublished([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetPublishedCoursesQuery(page, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // GET COURSE BY ID
    // ================================================================
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetCourseByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET COURSES BY FACILITATOR
    // ================================================================
    [HttpGet("facilitator/{facilitatorId}")]
    [Authorize(Roles = "Facilitator")]
    public async Task<IActionResult> GetByFacilitator(int facilitatorId)
    {
        var query = new GetCoursesByFacilitatorQuery(facilitatorId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // CREATE COURSE - FIXED
    // ================================================================
    [HttpPost]
    [Authorize(Roles = "SetaAdministrator,Facilitator")]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto request)
    {
        var command = new CreateCourseCommand(
            request.CourseName,
            request.FacilitatorId ?? 0,  // ✅ Convert int? to int
            request.SaqaId,
            request.CurriculumCode,
            request.NqfLevel
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // UPDATE COURSE - FIXED
    // ================================================================
    [HttpPut("{id}")]
    [Authorize(Roles = "SetaAdministrator,Facilitator")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto request)
    {
        var command = new UpdateCourseCommand(
            id,
            request.CourseName,
            request.SaqaId,
            request.CurriculumCode,
            request.NqfLevel
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // DELETE COURSE
    // ================================================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteCourseCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // PUBLISH COURSE
    // ================================================================
    [HttpPost("{id}/publish")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Publish(int id)
    {
        var command = new PublishCourseCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // ARCHIVE COURSE
    // ================================================================
    [HttpPost("{id}/archive")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Archive(int id)
    {
        var command = new ArchiveCourseCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

