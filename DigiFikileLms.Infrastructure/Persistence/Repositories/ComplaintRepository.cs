using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

/// <summary>
/// TYPE: ComplaintRepository
/// PURPOSE: EF Core persistence for complaints. Reads include the complainant so handlers and the
///          controller never lazy-load; writes follow the async unit-of-work pattern of the other
///          repositories.
/// LMS ROLE: Supports the Complaint area while respecting the Infrastructure layer boundary.
/// </summary>
public class ComplaintRepository : IComplaintRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public ComplaintRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Complaint?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _context.Complaints
            .AsNoTracking()
            .Include(c => c.Complainant)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IEnumerable<Complaint>> GetAllAsync(ComplaintStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Complaints
            .AsNoTracking()
            .Include(c => c.Complainant)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Complaint>> GetByComplainantAsync(int complainantUserId, CancellationToken cancellationToken = default) =>
        await _context.Complaints
            .AsNoTracking()
            .Include(c => c.Complainant)
            .Where(c => c.ComplainantUserId == complainantUserId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Complaint complaint, CancellationToken cancellationToken = default) =>
        await _context.Complaints.AddAsync(complaint, cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}