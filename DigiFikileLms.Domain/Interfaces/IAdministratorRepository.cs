using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;

namespace DigiFikileLms.Domain.Interfaces;

public interface IAdministratorRepository
{
    Task<SetaAdministrator?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SetaAdministrator?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SetaAdministrator>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SetaAdministrator>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SetaAdministrator administrator, CancellationToken cancellationToken = default);
    Task UpdateAsync(SetaAdministrator administrator, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}