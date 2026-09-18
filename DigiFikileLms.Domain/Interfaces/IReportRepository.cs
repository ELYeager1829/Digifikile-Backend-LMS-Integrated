using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Report>> GetBySetaAdministratorAsync(int setaAdministratorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Report>> GetByTrainingProviderAsync(int trainingProviderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Report>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Report report, CancellationToken cancellationToken = default);
    Task UpdateAsync(Report report, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

