using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface ISystemAdministratorRepository
{
    Task<SystemAdministrator?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SystemAdministrator?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemAdministrator>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SystemAdministrator admin, CancellationToken cancellationToken = default);
    Task UpdateAsync(SystemAdministrator admin, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}