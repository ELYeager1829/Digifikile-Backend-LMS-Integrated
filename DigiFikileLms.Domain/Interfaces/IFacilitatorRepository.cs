using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IFacilitatorRepository
{
    Task<Facilitator?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Facilitator?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Facilitator?> GetByStaffNumberAsync(string staffNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Facilitator>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Facilitator?> GetWithCoursesAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Facilitator facilitator, CancellationToken cancellationToken = default);
    Task UpdateAsync(Facilitator facilitator, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

