using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public RoleRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _context.Roles.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Roles.Include(x => x.Permissions).OrderBy(x => x.Name).ToListAsync(cancellationToken);

    public async Task<Role?> GetWithPermissionsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Roles
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _context.Roles.AnyAsync(x => x.Name == name, cancellationToken);

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _context.Roles.AddAsync(role, cancellationToken);
    }

    public Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        _context.Roles.Update(role);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Roles.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}