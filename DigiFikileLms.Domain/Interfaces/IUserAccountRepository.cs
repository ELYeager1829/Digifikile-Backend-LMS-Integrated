// DigiFikileLms.Domain/Interfaces/IUserAccountRepository.cs
using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Domain.Interfaces;

public interface IUserAccountRepository
{
    Task<UserAccount?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserAccount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserAccount>> GetByRoleAsync(string role, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserAccount?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(UserAccount user, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserAccount user, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

