using Microsoft.EntityFrameworkCore;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public EnrollmentRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .ThenInclude(s => s!.User)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Include(e => e.Course)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetByCourseAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .Include(e => e.Student)
            .ThenInclude(s => s!.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);
    }

    public async Task<bool> IsStudentEnrolledAsync(int studentId, int courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);
    }

    // ✅ ADDED: Get all active enrollments
    public async Task<IEnumerable<Enrollment>> GetActiveEnrollmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .ThenInclude(s => s!.User)
            .Include(e => e.Course)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .ThenInclude(s => s!.User)
            .Include(e => e.Course)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
    }

    public Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        _context.Enrollments.Update(enrollment);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var enrollment = await GetByIdAsync(id, cancellationToken);
        if (enrollment != null)
        {
            _context.Enrollments.Remove(enrollment);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}