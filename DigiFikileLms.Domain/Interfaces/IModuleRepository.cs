using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IModuleRepository
{
    Task<Module?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Module>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Module?> GetWithAssessmentsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Module>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Module module, CancellationToken cancellationToken = default);
    Task UpdateAsync(Module module, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

