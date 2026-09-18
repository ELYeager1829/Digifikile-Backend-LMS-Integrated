namespace DigiFikileLms.Application.DTOs.Certificates;

public class CertificateDto
{
    public int Id { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime IssuedAt { get; set; }
    public string VerificationToken { get; set; } = string.Empty;
}

