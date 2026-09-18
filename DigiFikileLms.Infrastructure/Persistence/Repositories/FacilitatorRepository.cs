using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class FacilitatorRepository : IFacilitatorRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public FacilitatorRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Facilitator?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Facilitators.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Facilitator?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await _context.Facilitators.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task<Facilitator?> GetByStaffNumberAsync(string staffNumber, CancellationToken cancellationToken = default)
        => await _context.Facilitators.AsNoTracking().FirstOrDefaultAsync(x => x.StaffNumber == staffNumber, cancellationToken);

    public async Task<IEnumerable<Facilitator>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Facilitators.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Facilitator?> GetWithCoursesAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Facilitators
            .AsNoTracking()
            .Include(x => x.Courses)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Facilitator facilitator, CancellationToken cancellationToken = default)
    {
        await _context.Facilitators.AddAsync(facilitator, cancellationToken);
    }

    public Task UpdateAsync(Facilitator facilitator, CancellationToken cancellationToken = default)
    {
        _context.Facilitators.Update(facilitator);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Facilitators.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Facilitators.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
