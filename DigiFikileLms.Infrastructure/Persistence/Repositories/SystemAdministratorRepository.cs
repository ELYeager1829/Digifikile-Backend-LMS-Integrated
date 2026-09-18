using Microsoft.EntityFrameworkCore;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class SystemAdministratorRepository : ISystemAdministratorRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public SystemAdministratorRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<SystemAdministrator?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SystemAdministrators
            .Include(sa => sa.User)
            .FirstOrDefaultAsync(sa => sa.Id == id, cancellationToken);
    }

    public async Task<SystemAdministrator?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.SystemAdministrators
            .Include(sa => sa.User)
            .FirstOrDefaultAsync(sa => sa.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<SystemAdministrator>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SystemAdministrators
            .Include(sa => sa.User)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SystemAdministrator admin, CancellationToken cancellationToken = default)
    {
        await _context.SystemAdministrators.AddAsync(admin, cancellationToken);
    }

    public Task UpdateAsync(SystemAdministrator admin, CancellationToken cancellationToken = default)
    {
        _context.SystemAdministrators.Update(admin);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var admin = await GetByIdAsync(id, cancellationToken);
        if (admin != null)
        {
            _context.SystemAdministrators.Remove(admin);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}