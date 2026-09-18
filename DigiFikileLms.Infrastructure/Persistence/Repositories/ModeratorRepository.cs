using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class ModeratorRepository : IModeratorRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public ModeratorRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Moderator?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Moderators.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Moderator?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => await _context.Moderators.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task<Moderator?> GetByStaffNumberAsync(string staffNumber, CancellationToken cancellationToken = default)
        => await _context.Moderators.AsNoTracking().FirstOrDefaultAsync(x => x.StaffNumber == staffNumber, cancellationToken);

    public async Task<IEnumerable<Moderator>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Moderators.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Moderator?> GetWithFeedbackAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Moderators
            .AsNoTracking()
            .Include(x => x.Feedbacks)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Moderator moderator, CancellationToken cancellationToken = default)
    {
        await _context.Moderators.AddAsync(moderator, cancellationToken);
    }

    public Task UpdateAsync(Moderator moderator, CancellationToken cancellationToken = default)
    {
        _context.Moderators.Update(moderator);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Moderators.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Moderators.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}