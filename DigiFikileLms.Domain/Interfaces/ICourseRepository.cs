// DigiFikileLms.Domain/Interfaces/ICourseRepository.cs
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Course?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetByFacilitatorAsync(int facilitatorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetWithModulesAsync(CancellationToken cancellationToken = default);
    Task<Course?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Course course, CancellationToken cancellationToken = default);
    Task UpdateAsync(Course course, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

