using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface ISETAProgrammeRepository
{
    Task<SETAProgramme?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SETAProgramme>> GetBySetaAdministratorAsync(int setaAdministratorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SETAProgramme>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SETAProgramme programme, CancellationToken cancellationToken = default);
    Task UpdateAsync(SETAProgramme programme, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

