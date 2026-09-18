using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Complaint;
using DigiFikileLms.Domain.Enums;
using MediatR;

namespace DigiFikileLms.Application.Features.Complaints.Commands;

/// <summary>
/// Lays a new complaint on behalf of the authenticated caller. Every complaint starts Open and
/// only a System Administrator moves it forward.
/// </summary>
public record LayComplaintCommand(
    int ComplainantUserId,
    string Title,
    string Description,
    string? Category) : IRequest<BaseResponse<ComplaintDto>>;

/// <summary>
/// Moves a complaint to a new state with optional resolution notes.
/// </summary>
public record UpdateComplaintStatusCommand(
    int ComplaintId,
    ComplaintStatus Status,
    string? ResolutionNotes) : IRequest<BaseResponse<ComplaintDto>>;