using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Assessment;
using DigiFikileLms.Application.DTOs;
using MediatR;

namespace DigiFikileLms.Application.Features.Assessments.Commands;

// ================================================================
// COMMANDS
// ================================================================

public record CreateAssessmentCommand(
    int ModuleId,
    int FacilitatorId,
    string Title,
    string AssessmentType,
    decimal? MaxScore = null,
    decimal? PassingScore = null,
    DateTime? DueDate = null
) : IRequest<BaseResponse<DigiFikileLms.Application.DTOs.Assessment.AssessmentDto>>;  // ✅ Fully qualified

public record SubmitAssessmentCommand(
    int AssessmentId,
    int StudentId,
    object Answers
) : IRequest<BaseResponse<SubmissionDto>>;

public record GradeAssessmentCommand(
    int SubmissionId,
    int Percentage,
    string Grade,
    string? Feedback = null
) : IRequest<BaseResponse<ResultDto>>;

// ================================================================
// HANDLERS
// ================================================================

public class CreateAssessmentCommandHandler 
    : IRequestHandler<CreateAssessmentCommand, BaseResponse<DigiFikileLms.Application.DTOs.Assessment.AssessmentDto>>  // ✅ Fully qualified
{
    public Task<BaseResponse<DigiFikileLms.Application.DTOs.Assessment.AssessmentDto>> Handle(  // ✅ Fully qualified
        CreateAssessmentCommand request, 
        CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Create assessment via IAssessmentRepository.");
}

public class SubmitAssessmentCommandHandler 
    : IRequestHandler<SubmitAssessmentCommand, BaseResponse<SubmissionDto>>
{
    public Task<BaseResponse<SubmissionDto>> Handle(
        SubmitAssessmentCommand request, 
        CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Submit assessment via ISubmissionRepository.");
}

public class GradeAssessmentCommandHandler 
    : IRequestHandler<GradeAssessmentCommand, BaseResponse<ResultDto>>
{
    public Task<BaseResponse<ResultDto>> Handle(
        GradeAssessmentCommand request, 
        CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Grade assessment via IResultRepository.");
}

