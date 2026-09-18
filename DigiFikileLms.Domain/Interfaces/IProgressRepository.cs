// DigiFikileLms.Domain/Interfaces/IProgressRepository.cs
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IProgressRepository
{
    Task<Progress?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Progress>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Progress>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Progress?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    Task<decimal> GetOverallProgressAsync(int studentId, CancellationToken cancellationToken = default);
    Task AddAsync(Progress progress, CancellationToken cancellationToken = default);
    Task UpdateAsync(Progress progress, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}