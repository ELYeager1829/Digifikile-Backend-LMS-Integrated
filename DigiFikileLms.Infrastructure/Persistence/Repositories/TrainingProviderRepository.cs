using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class TrainingProviderRepository : ITrainingProviderRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public TrainingProviderRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<TrainingProvider?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.TrainingProviders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<TrainingProvider?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await _context.TrainingProviders.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task<TrainingProvider?> GetByCompanyNameAsync(string companyName, CancellationToken cancellationToken = default)
        => await _context.TrainingProviders.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyName == companyName, cancellationToken);

    public async Task<IEnumerable<TrainingProvider>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.TrainingProviders.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<TrainingProvider?> GetWithDepartmentsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.TrainingProviders
            .AsNoTracking()
            .Include(x => x.Departments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(TrainingProvider trainingProvider, CancellationToken cancellationToken = default)
    {
        await _context.TrainingProviders.AddAsync(trainingProvider, cancellationToken);
    }

    public Task UpdateAsync(TrainingProvider trainingProvider, CancellationToken cancellationToken = default)
    {
        _context.TrainingProviders.Update(trainingProvider);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.TrainingProviders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.TrainingProviders.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
