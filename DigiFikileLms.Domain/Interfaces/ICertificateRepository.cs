// DigiFikileLms.Domain/Interfaces/ICertificateRepository.cs
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface ICertificateRepository
{
    Task<Certificate?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Certificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Certificate>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Certificate>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default);
    Task<bool> VerifyCertificateAsync(string certificateNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default);
    Task UpdateAsync(Certificate certificate, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}