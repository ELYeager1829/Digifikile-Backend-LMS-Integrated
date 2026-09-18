using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using MediatR;

namespace DigiFikileLms.Application.Features.Reports.Queries;

// ================================================================
// QUERIES
// ================================================================

public record GetTrainingCompletionReportQuery(
    int TrainingProviderId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<BaseResponse<TrainingCompletionReportDto>>;

public record GetAssessmentPerformanceReportQuery(
    int TrainingProviderId,
    int? CourseId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<BaseResponse<TrainingCompletionReportDto>>;

// ✅ Uses int (not Guid)
public record GetCourseAnalyticsQuery(
    int CourseId
) : IRequest<BaseResponse<ReportSummaryDto>>;

public record GetUserActivityReportQuery(
    DateTime From,
    DateTime To
) : IRequest<BaseResponse<ReportSummaryDto>>;

// ================================================================
// HANDLERS
// ================================================================

public class GetTrainingCompletionReportQueryHandler 
    : IRequestHandler<GetTrainingCompletionReportQuery, BaseResponse<TrainingCompletionReportDto>>
{
    public Task<BaseResponse<TrainingCompletionReportDto>> Handle(
        GetTrainingCompletionReportQuery request,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Generate training completion report.");
}

public class GetAssessmentPerformanceReportQueryHandler 
    : IRequestHandler<GetAssessmentPerformanceReportQuery, BaseResponse<TrainingCompletionReportDto>>
{
    public Task<BaseResponse<TrainingCompletionReportDto>> Handle(
        GetAssessmentPerformanceReportQuery request,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Generate assessment performance report.");
}

public class GetCourseAnalyticsQueryHandler 
    : IRequestHandler<GetCourseAnalyticsQuery, BaseResponse<ReportSummaryDto>>
{
    public Task<BaseResponse<ReportSummaryDto>> Handle(
        GetCourseAnalyticsQuery request,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Aggregate enrollment, completion, and assessment metrics.");
}

public class GetUserActivityReportQueryHandler 
    : IRequestHandler<GetUserActivityReportQuery, BaseResponse<ReportSummaryDto>>
{
    public Task<BaseResponse<ReportSummaryDto>> Handle(
        GetUserActivityReportQuery request,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Aggregate user activity over date range.");
}

