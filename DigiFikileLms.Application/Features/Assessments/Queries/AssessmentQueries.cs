using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Assessment;
using MediatR;

namespace DigiFikileLms.Application.Features.Assessments.Queries;

// ================================================================
// QUERIES
// ================================================================

public record GetAssessmentByIdQuery(
    int Id
) : IRequest<BaseResponse<AssessmentDto>>;

public record GetAssessmentsByModuleQuery(
    int ModuleId
) : IRequest<BaseResponse<List<AssessmentDto>>>;

public record GetPendingAssessmentsQuery(
    int ModeratorId
) : IRequest<BaseResponse<List<AssessmentDto>>>;

// ================================================================
// HANDLERS
// ================================================================

public class GetAssessmentByIdQueryHandler 
    : IRequestHandler<GetAssessmentByIdQuery, BaseResponse<AssessmentDto>>
{
    public Task<BaseResponse<AssessmentDto>> Handle(
        GetAssessmentByIdQuery request, 
        CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Load assessment by ID.");
}

public class GetAssessmentsByModuleQueryHandler 
    : IRequestHandler<GetAssessmentsByModuleQuery, BaseResponse<List<AssessmentDto>>>
{
    public Task<BaseResponse<List<AssessmentDto>>> Handle(
        GetAssessmentsByModuleQuery request, 
        CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Load assessments by module.");
}

public class GetPendingAssessmentsQueryHandler 
    : IRequestHandler<GetPendingAssessmentsQuery, BaseResponse<List<AssessmentDto>>>
{
    public Task<BaseResponse<List<AssessmentDto>>> Handle(
        GetPendingAssessmentsQuery request, 
        CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Load pending assessments for moderator.");
}
