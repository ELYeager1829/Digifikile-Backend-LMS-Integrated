using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IResultRepository
{
    Task<Result?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Result>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Result>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Result result, CancellationToken cancellationToken = default);
    Task UpdateAsync(Result result, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

