using DigiFikileLms.Application.DTOs.SystemLog;
using DigiFikileLms.Application.Features.SystemLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/system-logs")]
[Authorize(Roles = "SystemAdministrator")]
public class SystemLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all system logs
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetAllSystemLogsQuery(page, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Search and filter system logs
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchLogs([FromQuery] SystemLogFilterDto filter)
    {
        var query = new SearchSystemLogsQuery(filter);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Export system logs as CSV
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> ExportLogs([FromQuery] SystemLogFilterDto filter)
    {
        var query = new ExportSystemLogsQuery(filter);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess || result.Data == null)
            return NotFound(result);

        var file = result.Data;
        return File(file.FileContent, file.ContentType, file.FileName);
    }
}