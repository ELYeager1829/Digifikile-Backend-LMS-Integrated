using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class ResultRepository : IResultRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public ResultRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Result?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Results.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Result>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default)
        => await _context.Results.AsNoTracking().Where(x => x.StudentId == studentId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Result>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Results.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Result result, CancellationToken cancellationToken = default)
    {
        await _context.Results.AddAsync(result, cancellationToken);
    }

    public Task UpdateAsync(Result result, CancellationToken cancellationToken = default)
    {
        _context.Results.Update(result);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Results.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Results.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
