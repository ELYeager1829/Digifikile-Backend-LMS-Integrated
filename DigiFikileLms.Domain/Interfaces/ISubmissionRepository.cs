using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Submission>> GetByAssessmentAsync(int assessmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Submission>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<Submission?> GetByStudentAndAssessmentAsync(int studentId, int assessmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Submission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Submission submission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

