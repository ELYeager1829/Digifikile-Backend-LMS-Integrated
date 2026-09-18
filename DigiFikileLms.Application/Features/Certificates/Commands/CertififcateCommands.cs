using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Certificates;
using MediatR;

namespace DigiFikileLms.Application.Features.Certificates;

public record IssueCertificateCommand(
    int ResultId,
    int TrainingProviderId,
    string CourseCode
) : IRequest<BaseResponse<CertificateDto>>;

public record RevokeCertificateCommand(
    int CertificateId
) : IRequest<BaseResponse<bool>>;

