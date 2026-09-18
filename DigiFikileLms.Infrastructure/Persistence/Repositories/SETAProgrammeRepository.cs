using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class SETAProgrammeRepository : ISETAProgrammeRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public SETAProgrammeRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<SETAProgramme?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.SETAProgrammes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<SETAProgramme>> GetBySetaAdministratorAsync(int setaAdministratorId, CancellationToken cancellationToken = default)
        => await _context.SETAProgrammes.AsNoTracking().Where(x => x.SetaAdministratorId == setaAdministratorId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<SETAProgramme>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.SETAProgrammes.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(SETAProgramme programme, CancellationToken cancellationToken = default)
    {
        await _context.SETAProgrammes.AddAsync(programme, cancellationToken);
    }

    public Task UpdateAsync(SETAProgramme programme, CancellationToken cancellationToken = default)
    {
        _context.SETAProgrammes.Update(programme);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.SETAProgrammes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.SETAProgrammes.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
