namespace DigiFikileLms.Application.DTOs.Students;

public class StudentCertificateDto
{
    public int CertificateId { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
}

