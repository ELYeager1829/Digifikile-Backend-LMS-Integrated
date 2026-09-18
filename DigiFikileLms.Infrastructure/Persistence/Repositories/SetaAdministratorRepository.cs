using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class SetaAdministratorRepository : ISetaAdministratorRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public SetaAdministratorRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<SetaAdministrator?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.SetaAdministrators.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<SetaAdministrator?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await _context.SetaAdministrators.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task<IEnumerable<SetaAdministrator>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.SetaAdministrators.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<SetaAdministrator>> GetActiveAsync(CancellationToken cancellationToken = default)
        => await _context.SetaAdministrators.AsNoTracking().Where(x => x.IsActive).ToListAsync(cancellationToken);

    public async Task AddAsync(SetaAdministrator setaAdministrator, CancellationToken cancellationToken = default)
    {
        await _context.SetaAdministrators.AddAsync(setaAdministrator, cancellationToken);
    }

    public Task UpdateAsync(SetaAdministrator setaAdministrator, CancellationToken cancellationToken = default)
    {
        _context.SetaAdministrators.Update(setaAdministrator);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.SetaAdministrators.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.SetaAdministrators.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}