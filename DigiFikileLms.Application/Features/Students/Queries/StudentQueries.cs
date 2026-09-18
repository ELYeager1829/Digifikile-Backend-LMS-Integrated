using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Students;
//using DigiFikileLms.Application.DTOs.Enrollment;
using MediatR;
using DigiFikileLms.Application.DTOs;

namespace DigiFikileLms.Application.Features.Students;

public record GetStudentProfileQuery(
    int UserId
) : IRequest<BaseResponse<StudentProfileDto>>;

public record GetStudentEnrollmentsQuery(
    int UserId
) : IRequest<BaseResponse<List<StudentEnrollmentDto>>>;

public record GetStudentProgressQuery(
    int UserId
) : IRequest<BaseResponse<List<StudentProgressDto>>>;

public record GetStudentCertificatesQuery(
    int UserId
) : IRequest<BaseResponse<List<StudentCertificateDto>>>;

public record GetAllStudentsQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<BaseResponse<PagedResponse<StudentProfileDto>>>;

public record GetStudentByIdQuery(
    int Id
) : IRequest<BaseResponse<StudentProfileDto>>;