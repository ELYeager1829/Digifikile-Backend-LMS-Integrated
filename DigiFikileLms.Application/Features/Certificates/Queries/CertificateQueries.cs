using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Certificates;
using DigiFikileLms.Application.DTOs.Students;
using MediatR;

namespace DigiFikileLms.Application.Features.Certificates.Queries;

public record GetStudentCertificatesQuery(
    int StudentId
) : IRequest<BaseResponse<List<StudentCertificateDto>>>;

public record VerifyCertificateQuery(
    string CertificateNumber
) : IRequest<BaseResponse<CertificateDto>>;

public record GetCertificateByIdQuery(
    int CertificateId
) : IRequest<BaseResponse<CertificateDto>>;