using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public FeedbackRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Feedback?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Feedbacks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Feedback>> GetByModeratorAsync(int moderatorId, CancellationToken cancellationToken = default)
        => await _context.Feedbacks.AsNoTracking().Where(x => x.ModeratorId == moderatorId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Feedback>> GetByResultAsync(int resultId, CancellationToken cancellationToken = default)
        => await _context.Feedbacks.AsNoTracking().Where(x => x.ResultId == resultId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Feedback>> GetByAssessmentAsync(int assessmentId, CancellationToken cancellationToken = default)
        => await _context.Feedbacks.AsNoTracking().Where(x => x.AssessmentId == assessmentId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Feedback>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Feedbacks.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Feedback feedback, CancellationToken cancellationToken = default)
    {
        await _context.Feedbacks.AddAsync(feedback, cancellationToken);
    }

    public Task UpdateAsync(Feedback feedback, CancellationToken cancellationToken = default)
    {
        _context.Feedbacks.Update(feedback);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Feedbacks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Feedbacks.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
