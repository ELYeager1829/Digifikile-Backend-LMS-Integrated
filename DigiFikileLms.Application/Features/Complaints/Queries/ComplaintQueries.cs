using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Complaint;
using MediatR;

namespace DigiFikileLms.Application.Features.Complaints.Queries;

/// <summary>
/// Lists every complaint in the system, newest first, optionally filtered by a ComplaintStatus
/// name. System Administrator only; the controller enforces the role.
/// </summary>
public record GetAllComplaintsQuery(string? Status = null) : IRequest<BaseResponse<List<ComplaintDto>>>;

/// <summary>
/// Fetches one complaint by id. System Administrator only.
/// </summary>
public record GetComplaintByIdQuery(int ComplaintId) : IRequest<BaseResponse<ComplaintDto>>;

/// <summary>
/// Lists the complaints the authenticated caller laid themselves, newest first.
/// </summary>
public record GetMyComplaintsQuery(int ComplainantUserId) : IRequest<BaseResponse<List<ComplaintDto>>>;