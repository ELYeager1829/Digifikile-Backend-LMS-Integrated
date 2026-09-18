using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public DepartmentRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Departments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Department>> GetByTrainingProviderAsync(int trainingProviderId, CancellationToken cancellationToken = default)
        => await _context.Departments.AsNoTracking().Where(x => x.TrainingProviderId == trainingProviderId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Departments.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        await _context.Departments.AddAsync(department, cancellationToken);
    }

    public Task UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Update(department);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Departments.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
