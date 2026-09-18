using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Certificates;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Certificates;

public class IssueCertificateCommandHandler 
    : IRequestHandler<IssueCertificateCommand, BaseResponse<CertificateDto>>
{
    private readonly ICertificateRepository _certificateRepository;
    private readonly IResultRepository _resultRepository;
    private readonly ITrainingProviderRepository _trainingProviderRepository;

    public IssueCertificateCommandHandler(
        ICertificateRepository certificateRepository,
        IResultRepository resultRepository,
        ITrainingProviderRepository trainingProviderRepository)
    {
        _certificateRepository = certificateRepository;
        _resultRepository = resultRepository;
        _trainingProviderRepository = trainingProviderRepository;
    }

    public async Task<BaseResponse<CertificateDto>> Handle(
        IssueCertificateCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _resultRepository.GetByIdAsync(request.ResultId, cancellationToken);
        if (result == null)
            return BaseResponse<CertificateDto>.Failure("Result not found");

        var trainingProvider = await _trainingProviderRepository.GetByIdAsync(
            request.TrainingProviderId, cancellationToken);
        if (trainingProvider == null)
            return BaseResponse<CertificateDto>.Failure("Training provider not found");

        var certificate = Certificate.Create(result, request.CourseCode);
        await _certificateRepository.AddAsync(certificate, cancellationToken);
        await _certificateRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<CertificateDto>.Success(new CertificateDto
        {
            Id = certificate.Id,
            CertificateNumber = certificate.CertificateNumber ?? string.Empty,
            CourseCode = certificate.CourseCode ?? string.Empty,
            Description = certificate.Description,
            IssuedAt = certificate.IssuedAt,
            VerificationToken = certificate.VerificationToken!  // ✅ Use null-forgiving operator
        });
    }
}

