using Microsoft.EntityFrameworkCore;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class AssessmentRepository : IAssessmentRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public AssessmentRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Assessment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Assessments
            .Include(a => a.Module)
            .Include(a => a.Student)
            .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Assessment>> GetByModuleAsync(int moduleId, CancellationToken cancellationToken = default)
    {
        return await _context.Assessments
            .Where(a => a.ModuleId == moduleId)
            .Include(a => a.Student)
            .ThenInclude(s => s!.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Assessment>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _context.Assessments
            .Where(a => a.StudentId == studentId)
            .Include(a => a.Module)
            .ToListAsync(cancellationToken);
    }

    public async Task<Assessment?> GetWithSubmissionsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Assessments
            .Include(a => a.Submissions)
            .Include(a => a.Module)
            .Include(a => a.Student)
            .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    // ✅ ADDED: Get pending assessments (not yet graded)
    public async Task<IEnumerable<Assessment>> GetPendingAssessmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Assessments
            .Where(a => a.Grades == null)
            .Include(a => a.Module)
            .Include(a => a.Student)
            .ThenInclude(s => s!.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Assessment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Assessments
            .Include(a => a.Module)
            .Include(a => a.Student)
            .ThenInclude(s => s!.User)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Assessment assessment, CancellationToken cancellationToken = default)
    {
        await _context.Assessments.AddAsync(assessment, cancellationToken);
    }

    public Task UpdateAsync(Assessment assessment, CancellationToken cancellationToken = default)
    {
        _context.Assessments.Update(assessment);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var assessment = await GetByIdAsync(id, cancellationToken);
        if (assessment != null)
        {
            _context.Assessments.Remove(assessment);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}