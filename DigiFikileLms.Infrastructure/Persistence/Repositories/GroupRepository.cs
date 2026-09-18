using Microsoft.EntityFrameworkCore;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public GroupRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.setaAdministrator)
                .ThenInclude(a => a.User)
            .Include(g => g.setaProgramme)
            .Include(g => g.GroupEnrollments)
                .ThenInclude(ge => ge.Student)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Group>> GetByAdministratorAsync(int administratorId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Where(g => g.setaAdministratorId == administratorId)
            .Include(g => g.GroupEnrollments)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Group>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.setaAdministrator)
                .ThenInclude(a => a.User)
            .Include(g => g.GroupEnrollments)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
    {
        await _context.Groups.AddAsync(group, cancellationToken);
    }

    public Task UpdateAsync(Group group, CancellationToken cancellationToken = default)
    {
        _context.Groups.Update(group);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var group = await GetByIdAsync(id, cancellationToken);
        if (group != null)
        {
            _context.Groups.Remove(group);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}