using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IFeedbackRepository
{
    Task<Feedback?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Feedback>> GetByModeratorAsync(int moderatorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Feedback>> GetByResultAsync(int resultId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Feedback>> GetByAssessmentAsync(int assessmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Feedback>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Feedback feedback, CancellationToken cancellationToken = default);
    Task UpdateAsync(Feedback feedback, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

