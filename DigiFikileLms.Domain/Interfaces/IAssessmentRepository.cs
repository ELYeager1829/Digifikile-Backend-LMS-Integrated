// DigiFikileLms.Domain/Interfaces/IAssessmentRepository.cs
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IAssessmentRepository
{
    Task<Assessment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assessment>> GetByModuleAsync(int moduleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assessment>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<Assessment?> GetWithSubmissionsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assessment>> GetPendingAssessmentsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Assessment assessment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Assessment assessment, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}