using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;

namespace DigiFikileLms.Domain.Interfaces;

public interface IComplaintRepository
{
    Task<Complaint?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Complaint>> GetAllAsync(ComplaintStatus? status = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Complaint>> GetByComplainantAsync(int complainantUserId, CancellationToken cancellationToken = default);
    Task AddAsync(Complaint complaint, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}