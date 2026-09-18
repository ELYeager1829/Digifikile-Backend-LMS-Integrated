using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public ModuleRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Module?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Modules.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Module>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default)
        => await _context.Modules.AsNoTracking().Where(x => x.CourseId == courseId).ToListAsync(cancellationToken);

    public async Task<Module?> GetWithAssessmentsAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Modules
            .AsNoTracking()
            .Include(x => x.Assessments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Module>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Modules.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Module module, CancellationToken cancellationToken = default)
    {
        await _context.Modules.AddAsync(module, cancellationToken);
    }

    public Task UpdateAsync(Module module, CancellationToken cancellationToken = default)
    {
        _context.Modules.Update(module);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Modules.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Modules.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
