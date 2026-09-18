using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using MediatR;

namespace DigiFikileLms.Application.Features.Enrollments;

public record GetStudentEnrollmentsQuery(
    int StudentId
) : IRequest<BaseResponse<List<StudentEnrollmentDto>>>;

public record GetCourseEnrollmentsQuery(
    int CourseId
) : IRequest<BaseResponse<List<EnrollmentDetailDto>>>;