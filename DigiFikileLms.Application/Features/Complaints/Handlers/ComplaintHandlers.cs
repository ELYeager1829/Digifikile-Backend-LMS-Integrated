using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Complaint;
using DigiFikileLms.Application.Features.Complaints.Commands;
using DigiFikileLms.Application.Features.Complaints.Queries;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Complaints;

// ================================================================
// COMPLAINT HANDLERS
// ================================================================
// One use case per handler. Validation that maps to a 400 lives inside the handlers (mirroring
// the SystemAdmin area) so the response contract stays a BaseResponse, and the complainant id
// always comes from the authenticated caller, never from the request body.

public class LayComplaintCommandHandler
    : IRequestHandler<LayComplaintCommand, BaseResponse<ComplaintDto>>
{
    private readonly IComplaintRepository _complaintRepository;
    private readonly IUserAccountRepository _userRepository;

    public LayComplaintCommandHandler(
        IComplaintRepository complaintRepository,
        IUserAccountRepository userRepository)
    {
        _complaintRepository = complaintRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<ComplaintDto>> Handle(
        LayComplaintCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ComplainantUserId <= 0)
            return BaseResponse<ComplaintDto>.Failure("An authenticated caller is required");

        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length > 200)
            return BaseResponse<ComplaintDto>.Failure("A title of at most 200 characters is required");

        if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Trim().Length > 4000)
            return BaseResponse<ComplaintDto>.Failure("A description of at most 4000 characters is required");

        if (!string.IsNullOrWhiteSpace(request.Category) && request.Category.Trim().Length > 100)
            return BaseResponse<ComplaintDto>.Failure("Category must not exceed 100 characters");

        var complainant = await _userRepository.GetByIdAsync(request.ComplainantUserId, cancellationToken);
        if (complainant == null)
            return BaseResponse<ComplaintDto>.Failure("Complainant account not found");

        var complaint = Complaint.Create(
            request.ComplainantUserId,
            request.Title,
            request.Description,
            request.Category);

        await _complaintRepository.AddAsync(complaint, cancellationToken);
        await _complaintRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<ComplaintDto>.Success(ComplaintMapper.ToDto(complaint, complainant));
    }
}

public class UpdateComplaintStatusCommandHandler
    : IRequestHandler<UpdateComplaintStatusCommand, BaseResponse<ComplaintDto>>
{
    private readonly IComplaintRepository _complaintRepository;

    public UpdateComplaintStatusCommandHandler(IComplaintRepository complaintRepository)
    {
        _complaintRepository = complaintRepository;
    }

    public async Task<BaseResponse<ComplaintDto>> Handle(
        UpdateComplaintStatusCommand request,
        CancellationToken cancellationToken)
    {
        var complaint = await _complaintRepository.GetByIdAsync(request.ComplaintId, cancellationToken);
        if (complaint == null)
            return BaseResponse<ComplaintDto>.Failure("Complaint not found");

        complaint.UpdateStatus(request.Status, request.ResolutionNotes);
        await _complaintRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<ComplaintDto>.Success(ComplaintMapper.ToDto(complaint, complaint.Complainant));
    }
}

public class GetAllComplaintsQueryHandler
    : IRequestHandler<GetAllComplaintsQuery, BaseResponse<List<ComplaintDto>>>
{
    private readonly IComplaintRepository _complaintRepository;

    public GetAllComplaintsQueryHandler(IComplaintRepository complaintRepository)
    {
        _complaintRepository = complaintRepository;
    }

    public async Task<BaseResponse<List<ComplaintDto>>> Handle(
        GetAllComplaintsQuery request,
        CancellationToken cancellationToken)
    {
        ComplaintStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<ComplaintStatus>(request.Status, ignoreCase: true, out var parsed) ||
                !Enum.IsDefined(parsed))
            {
                return BaseResponse<List<ComplaintDto>>.Failure(
                    "Status must be one of: Open, InReview, Resolved, Dismissed");
            }

            status = parsed;
        }

        var complaints = await _complaintRepository.GetAllAsync(status, cancellationToken);
        return BaseResponse<List<ComplaintDto>>.Success(
            complaints.Select(c => ComplaintMapper.ToDto(c, c.Complainant)).ToList());
    }
}

public class GetComplaintByIdQueryHandler
    : IRequestHandler<GetComplaintByIdQuery, BaseResponse<ComplaintDto>>
{
    private readonly IComplaintRepository _complaintRepository;

    public GetComplaintByIdQueryHandler(IComplaintRepository complaintRepository)
    {
        _complaintRepository = complaintRepository;
    }

    public async Task<BaseResponse<ComplaintDto>> Handle(
        GetComplaintByIdQuery request,
        CancellationToken cancellationToken)
    {
        var complaint = await _complaintRepository.GetByIdAsync(request.ComplaintId, cancellationToken);
        if (complaint == null)
            return BaseResponse<ComplaintDto>.Failure("Complaint not found");

        return BaseResponse<ComplaintDto>.Success(ComplaintMapper.ToDto(complaint, complaint.Complainant));
    }
}

public class GetMyComplaintsQueryHandler
    : IRequestHandler<GetMyComplaintsQuery, BaseResponse<List<ComplaintDto>>>
{
    private readonly IComplaintRepository _complaintRepository;

    public GetMyComplaintsQueryHandler(IComplaintRepository complaintRepository)
    {
        _complaintRepository = complaintRepository;
    }

    public async Task<BaseResponse<List<ComplaintDto>>> Handle(
        GetMyComplaintsQuery request,
        CancellationToken cancellationToken)
    {
        var complaints = await _complaintRepository.GetByComplainantAsync(request.ComplainantUserId, cancellationToken);
        return BaseResponse<List<ComplaintDto>>.Success(
            complaints.Select(c => ComplaintMapper.ToDto(c, c.Complainant)).ToList());
    }
}

/// <summary>
/// Copies complaint fields into the transport DTO. Kept internal: it is an implementation detail
/// of the complaint handlers.
/// </summary>
internal static class ComplaintMapper
{
    public static ComplaintDto ToDto(Complaint complaint, UserAccount complainant) => new()
    {
        Id = complaint.Id,
        ComplainantUserId = complaint.ComplainantUserId,
        ComplainantName = $"{complainant.Name} {complainant.Surname}".Trim(),
        ComplainantEmail = complainant.Email,
        Title = complaint.Title,
        Description = complaint.Description,
        Category = complaint.Category,
        Status = complaint.Status.ToString(),
        ResolutionNotes = complaint.ResolutionNotes,
        ResolvedAt = complaint.ResolvedAt,
        CreatedAt = complaint.CreatedAt
    };
}