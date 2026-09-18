using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface ISetaAdministratorRepository
{
    Task<SetaAdministrator?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SetaAdministrator?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SetaAdministrator>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SetaAdministrator>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SetaAdministrator setaAdministrator, CancellationToken cancellationToken = default);
    Task UpdateAsync(SetaAdministrator setaAdministrator, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}