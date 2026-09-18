using DigiFikileLms.Application.DTOs.Assessment;
using DigiFikileLms.Application.Features.Assessments.Commands;
using DigiFikileLms.Application.Features.Assessments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssessmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssessmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // GET ASSESSMENT BY ID
    // ================================================================
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetAssessmentByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET ASSESSMENTS BY MODULE
    // ================================================================
    [HttpGet("module/{moduleId}")]
    [Authorize]
    public async Task<IActionResult> GetByModule(int moduleId)
    {
        var query = new GetAssessmentsByModuleQuery(moduleId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET PENDING ASSESSMENTS (for Moderator)
    // ================================================================
    [HttpGet("pending/{moderatorId}")]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> GetPending(int moderatorId)
    {
        var query = new GetPendingAssessmentsQuery(moderatorId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // CREATE ASSESSMENT
    // ================================================================
    [HttpPost]
    [Authorize(Roles = "SetaAdministrator,Facilitator")]
    public async Task<IActionResult> Create([FromBody] CreateAssessmentDto request)
    {
        var command = new CreateAssessmentCommand(
            request.ModuleId,
            request.FacilitatorId,
            request.Title,
            request.AssessmentType,
            request.MaxScore,
            request.PassingScore,
            request.DueDate
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // SUBMIT ASSESSMENT
    // ================================================================
    [HttpPost("{id}/submit")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Submit(int id, [FromBody] SubmitAssessmentDto request)
    {
        var command = new SubmitAssessmentCommand(id, request.StudentId, request.Answers);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // GRADE ASSESSMENT
    // ================================================================
    [HttpPost("{id}/grade")]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> Grade(int id, [FromBody] GradeAssessmentDto request)
    {
        var command = new GradeAssessmentCommand(request.SubmissionId, request.Percentage, request.Grade, request.Feedback);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

