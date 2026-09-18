using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Certificate : BaseEntity
{
    private Certificate() { }

    public static Certificate Create(
        Result result,
        string? courseCode = null,
        string? description = null)
    {
        return new Certificate
        {
            ResultId = result.Id,
            Result = result,
            CourseCode = courseCode,
            Description = description,
            IssuedAt = DateTime.UtcNow,
            CertificateNumber = GenerateCertificateNumber(),
            VerificationToken = GenerateVerificationToken()  // ✅ ADD THIS
        };
    }

    private static string GenerateCertificateNumber()
    {
        return $"DFL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    private static string GenerateVerificationToken()  // ✅ ADD THIS METHOD
    {
        return Guid.NewGuid().ToString().ToUpper();
    }

    public int ResultId { get; private set; }
    public string? CourseCode { get; private set; }
    public string? Description { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public string CertificateNumber { get; private set; } = string.Empty;
    public string VerificationToken { get; private set; } = string.Empty;  // ✅ ADD THIS PROPERTY

    public virtual Result Result { get; private set; } = null!;
}


