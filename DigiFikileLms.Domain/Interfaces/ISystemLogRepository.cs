using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface ISystemLogRepository
{
    Task<SystemLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemLog>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemLog>> SearchAsync(string? action, string? resourceType, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task AddAsync(SystemLog log, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}