using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IModeratorRepository
{
    Task<Moderator?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Moderator?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Moderator?> GetByStaffNumberAsync(string staffNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Moderator>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Moderator?> GetWithFeedbackAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Moderator moderator, CancellationToken cancellationToken = default);
    Task UpdateAsync(Moderator moderator, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}