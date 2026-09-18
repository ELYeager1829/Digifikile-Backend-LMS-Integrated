using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IGroupEnrollmentRepository
{
    Task<GroupEnrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GroupEnrollment>> GetByGroupAsync(int groupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<GroupEnrollment>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task AddAsync(GroupEnrollment enrollment, CancellationToken cancellationToken = default);
    Task UpdateAsync(GroupEnrollment enrollment, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}