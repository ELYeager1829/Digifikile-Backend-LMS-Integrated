using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class ProgressRepository : IProgressRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public ProgressRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Progress?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.ProgressRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Progress>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default)
        => await _context.ProgressRecords.AsNoTracking().Where(x => x.StudentId == studentId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Progress>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default)
        => await _context.ProgressRecords.AsNoTracking().Where(x => x.CourseId == courseId).ToListAsync(cancellationToken);

    public async Task<Progress?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default)
        => await _context.ProgressRecords.AsNoTracking().FirstOrDefaultAsync(x => x.StudentId == studentId && x.CourseId == courseId, cancellationToken);

    public async Task<decimal> GetOverallProgressAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var progresses = await _context.ProgressRecords.AsNoTracking().Where(x => x.StudentId == studentId).ToListAsync(cancellationToken);
        if (!progresses.Any()) return 0m;
        return progresses.Average(x => x.Percentage);
    }

    public async Task<IEnumerable<Progress>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.ProgressRecords.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Progress progress, CancellationToken cancellationToken = default)
    {
        await _context.ProgressRecords.AddAsync(progress, cancellationToken);
    }

    public Task UpdateAsync(Progress progress, CancellationToken cancellationToken = default)
    {
        _context.ProgressRecords.Update(progress);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProgressRecords.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.ProgressRecords.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
