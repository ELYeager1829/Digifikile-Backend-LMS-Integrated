// DigiFikileLms.Infrastructure/Persistence/Repositories/CourseRepository.cs
using Microsoft.EntityFrameworkCore;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public CourseRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Facilitator)
            .ThenInclude(f => f!.User)
            .Include(c => c.Modules)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Facilitator)
            .ThenInclude(f => f!.User)
            .Include(c => c.Modules)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetByFacilitatorAsync(int facilitatorId, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Where(c => c.FacilitatorId == facilitatorId)
            .Include(c => c.Modules)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Where(c => c.Enrollments.Any(e => e.StudentId == studentId))
            .Include(c => c.Modules)
            .ToListAsync(cancellationToken);
    }

    // ✅ FIXED: Matches interface exactly
    public async Task<IEnumerable<Course>> GetWithModulesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Modules)
            .ThenInclude(m => m.Assessments)
            .ToListAsync(cancellationToken);
    }

    public async Task<Course?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Facilitator)
            .ThenInclude(f => f!.User)
            .Include(c => c.Modules)
            .ThenInclude(m => m.Assessments)
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.Student)
            .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .AnyAsync(c => c.Code == code, cancellationToken);
    }

    public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        await _context.Courses.AddAsync(course, cancellationToken);
    }

    public Task UpdateAsync(Course course, CancellationToken cancellationToken = default)
    {
        _context.Courses.Update(course);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await GetByIdAsync(id, cancellationToken);
        if (course != null)
        {
            _context.Courses.Remove(course);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}