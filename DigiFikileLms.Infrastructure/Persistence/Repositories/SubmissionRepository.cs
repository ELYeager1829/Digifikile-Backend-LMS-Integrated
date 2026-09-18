using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public SubmissionRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Submission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Submissions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Submission>> GetByAssessmentAsync(int assessmentId, CancellationToken cancellationToken = default)
        => await _context.Submissions.AsNoTracking().Where(x => x.AssessmentId == assessmentId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Submission>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default)
        => await _context.Submissions.AsNoTracking().Where(x => x.StudentId == studentId).ToListAsync(cancellationToken);

    public async Task<Submission?> GetByStudentAndAssessmentAsync(int studentId, int assessmentId, CancellationToken cancellationToken = default)
        => await _context.Submissions.AsNoTracking().FirstOrDefaultAsync(x => x.StudentId == studentId && x.AssessmentId == assessmentId, cancellationToken);

    public async Task<IEnumerable<Submission>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Submissions.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        await _context.Submissions.AddAsync(submission, cancellationToken);
    }

    public Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        _context.Submissions.Update(submission);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Submissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Submissions.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
