// DigiFikileLms.Domain/Interfaces/IEnrollmentRepository.cs
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    Task<bool> IsStudentEnrolledAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetActiveEnrollmentsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}