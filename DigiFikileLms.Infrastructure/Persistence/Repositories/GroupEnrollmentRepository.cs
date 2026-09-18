using Microsoft.EntityFrameworkCore;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class GroupEnrollmentRepository : IGroupEnrollmentRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public GroupEnrollmentRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<GroupEnrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.GroupEnrollments
            .Include(ge => ge.Group)
            .Include(ge => ge.Student)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(ge => ge.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<GroupEnrollment>> GetByGroupAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupEnrollments
            .Where(ge => ge.GroupId == groupId)
            .Include(ge => ge.Student)
                .ThenInclude(s => s.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<GroupEnrollment>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupEnrollments
            .Where(ge => ge.StudentId == studentId)
            .Include(ge => ge.Group)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(GroupEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _context.GroupEnrollments.AddAsync(enrollment, cancellationToken);
    }

    public Task UpdateAsync(GroupEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        _context.GroupEnrollments.Update(enrollment);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var enrollment = await GetByIdAsync(id, cancellationToken);
        if (enrollment != null)
        {
            _context.GroupEnrollments.Remove(enrollment);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}