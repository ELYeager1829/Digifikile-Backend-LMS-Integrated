using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public ReportRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Report?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Reports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Report>> GetBySetaAdministratorAsync(int setaAdministratorId, CancellationToken cancellationToken = default)
        => await _context.Reports.AsNoTracking().Where(x => x.SetaAdministratorId == setaAdministratorId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Report>> GetByTrainingProviderAsync(int trainingProviderId, CancellationToken cancellationToken = default)
        => await _context.Reports.AsNoTracking().Where(x => x.TrainingProviderId == trainingProviderId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Report>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Reports.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Report report, CancellationToken cancellationToken = default)
    {
        await _context.Reports.AddAsync(report, cancellationToken);
    }

    public Task UpdateAsync(Report report, CancellationToken cancellationToken = default)
    {
        _context.Reports.Update(report);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Reports.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Reports.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
