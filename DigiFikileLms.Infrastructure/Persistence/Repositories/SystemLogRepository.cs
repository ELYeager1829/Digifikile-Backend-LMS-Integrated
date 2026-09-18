using Microsoft.EntityFrameworkCore;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class SystemLogRepository : ISystemLogRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public SystemLogRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<SystemLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SystemLogs
            .Include(sl => sl.SystemAdministrator)
                .ThenInclude(sa => sa.User)
            .FirstOrDefaultAsync(sl => sl.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SystemLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SystemLogs
            .Include(sl => sl.SystemAdministrator)
                .ThenInclude(sa => sa.User)
            .OrderByDescending(sl => sl.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SystemLog>> SearchAsync(
        string? action,
        string? resourceType,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SystemLogs
            .Include(sl => sl.SystemAdministrator)
                .ThenInclude(sa => sa.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(action))
            query = query.Where(sl => sl.Action == action);

        if (!string.IsNullOrEmpty(resourceType))
            query = query.Where(sl => sl.ResourceType == resourceType);

        if (startDate.HasValue)
            query = query.Where(sl => sl.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(sl => sl.CreatedAt <= endDate.Value);

        return await query
            .OrderByDescending(sl => sl.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SystemLog log, CancellationToken cancellationToken = default)
    {
        await _context.SystemLogs.AddAsync(log, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}