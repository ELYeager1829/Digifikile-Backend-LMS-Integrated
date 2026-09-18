using Microsoft.EntityFrameworkCore;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class AdministratorRepository : IAdministratorRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public AdministratorRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<SetaAdministrator?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SetaAdministrators
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<SetaAdministrator?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.SetaAdministrators
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<SetaAdministrator>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SetaAdministrators
            .Include(a => a.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SetaAdministrator>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SetaAdministrators
            .Where(a => a.IsActive)
            .Include(a => a.User)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SetaAdministrator administrator, CancellationToken cancellationToken = default)
    {
        await _context.SetaAdministrators.AddAsync(administrator, cancellationToken);
    }

    public Task UpdateAsync(SetaAdministrator administrator, CancellationToken cancellationToken = default)
    {
        _context.SetaAdministrators.Update(administrator);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var admin = await GetByIdAsync(id, cancellationToken);
        if (admin != null)
        {
            _context.SetaAdministrators.Remove(admin);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}