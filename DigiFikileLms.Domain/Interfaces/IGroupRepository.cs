using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetByAdministratorAsync(int administratorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Group>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Group group, CancellationToken cancellationToken = default);
    Task UpdateAsync(Group group, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}