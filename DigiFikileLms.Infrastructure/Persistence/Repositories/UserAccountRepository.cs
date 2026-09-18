using Microsoft.EntityFrameworkCore;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class UserAccountRepository : IUserAccountRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public UserAccountRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<UserAccount?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccounts
            .Include(u => u.Student)
            .Include(u => u.Facilitator)
            .Include(u => u.Moderator)
            .Include(u => u.SetaAdministrator)
            .Include(u => u.TrainingProvider)
            .Include(u => u.Role)
                .ThenInclude(r => r!.Permissions)
            .Include(u => u.SystemAdministrator)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccounts
            .Include(u => u.SetaAdministrator)
            .Include(u => u.SystemAdministrator)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<UserAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserAccounts
            .Include(u => u.Student)
            .Include(u => u.Facilitator)
            .Include(u => u.Role)
                .ThenInclude(r => r!.Permissions)
            .ToListAsync(cancellationToken);
    }

    // ADDED: Get users by role
    public async Task<IEnumerable<UserAccount>> GetByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccounts
            .Where(u => u.UserRole.ToString() == role)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccounts
            .AnyAsync(u => u.Email.ToLower() == email.Trim().ToLower(), cancellationToken);
    }

    //  ADDED: Get user with all details
    public async Task<UserAccount?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccounts
            .Include(u => u.Student)
            .Include(u => u.Facilitator)
            .Include(u => u.Moderator)
            .Include(u => u.SetaAdministrator)
            .Include(u => u.TrainingProvider)
            .Include(u => u.Notifications)
            .Include(u => u.Role)
                .ThenInclude(r => r!.Permissions)
            .Include(u => u.SystemAdministrator)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AddAsync(UserAccount user, CancellationToken cancellationToken = default)
    {
        await _context.UserAccounts.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(UserAccount user, CancellationToken cancellationToken = default)
    {
        _context.UserAccounts.Update(user);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user != null)
        {
            _context.UserAccounts.Remove(user);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
