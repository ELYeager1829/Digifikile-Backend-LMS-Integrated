using DigiFikileLms.Application.Features.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // TRAINING COMPLETION REPORT
    // ================================================================
    [HttpGet("completion/{trainingProviderId}")]
    [Authorize(Roles = "SetaAdministrator,TrainingProvider")]
    public async Task<IActionResult> GetTrainingCompletion(
        int trainingProviderId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = new GetTrainingCompletionReportQuery(
            trainingProviderId,
            startDate,
            endDate
        );
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // ASSESSMENT PERFORMANCE REPORT
    // ================================================================
    [HttpGet("assessment-performance/{trainingProviderId}")]
    [Authorize(Roles = "SetaAdministrator,TrainingProvider")]
    public async Task<IActionResult> GetAssessmentPerformance(
        int trainingProviderId,
        [FromQuery] int? courseId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = new GetAssessmentPerformanceReportQuery(
            trainingProviderId,
            courseId,
            startDate,
            endDate
        );
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // COURSE ANALYTICS REPORT
    // ================================================================
    [HttpGet("course-analytics/{courseId}")]
    [Authorize(Roles = "SetaAdministrator,TrainingProvider")]
    public async Task<IActionResult> GetCourseAnalytics(int courseId)
    {
        var query = new GetCourseAnalyticsQuery(courseId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // USER ACTIVITY REPORT
    // ================================================================
    [HttpGet("user-activity")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> GetUserActivity(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var query = new GetUserActivityReportQuery(from, to);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

