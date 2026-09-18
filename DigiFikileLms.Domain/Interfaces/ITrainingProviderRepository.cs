using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface ITrainingProviderRepository
{
    Task<TrainingProvider?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TrainingProvider?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<TrainingProvider?> GetByCompanyNameAsync(string companyName, CancellationToken cancellationToken = default);
    Task<IEnumerable<TrainingProvider>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TrainingProvider?> GetWithDepartmentsAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(TrainingProvider trainingProvider, CancellationToken cancellationToken = default);
    Task UpdateAsync(TrainingProvider trainingProvider, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

