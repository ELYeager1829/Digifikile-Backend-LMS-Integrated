using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public PermissionRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Permissions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await _context.Permissions.AsNoTracking().FirstOrDefaultAsync(x => x.Code == code, cancellationToken);

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
       => await _context.Permissions.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    public async Task<IEnumerable<Permission>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
        => await _context.Permissions
            .AsNoTracking()
            .Where(x => x.Category == category)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _context.Permissions.AnyAsync(x => x.Name == name, cancellationToken);


    public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Permissions.AsNoTracking().OrderBy(x => x.Code).ToListAsync(cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        => await _context.Permissions.AnyAsync(x => x.Code == code, cancellationToken);

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        await _context.Permissions.AddAsync(permission, cancellationToken);
    }

    public Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _context.Permissions.Update(permission);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Permissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Permissions.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}