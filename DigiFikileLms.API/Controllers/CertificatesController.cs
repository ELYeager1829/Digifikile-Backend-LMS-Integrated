using DigiFikileLms.Application.DTOs.Certificates;
using DigiFikileLms.Application.DTOs.Students;
using DigiFikileLms.Application.Features.Certificates;
using MediatR;
using DigiFikileLms.Application.Features.Certificates.Queries;  
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CertificatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // GET STUDENT CERTIFICATES
    // ================================================================
    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "Student,SetaAdministrator,TrainingProvider")]
    public async Task<IActionResult> GetByStudent(int studentId)
    {
        var query = new GetStudentCertificatesQuery(studentId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET CERTIFICATE BY ID
    // ================================================================
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetCertificateByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // VERIFY CERTIFICATE
    // ================================================================
    [HttpGet("verify/{certificateNumber}")]
    [AllowAnonymous]
    public async Task<IActionResult> Verify(string certificateNumber)
    {
        var query = new VerifyCertificateQuery(certificateNumber);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // ISSUE CERTIFICATE
    // ================================================================
    [HttpPost]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Issue([FromBody] IssueCertificateDto request)
    {
        var command = new IssueCertificateCommand(
            request.ResultId,
            request.TrainingProviderId,
            request.CourseCode
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // REVOKE CERTIFICATE
    // ================================================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Revoke(int id)
    {
        var command = new RevokeCertificateCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}

