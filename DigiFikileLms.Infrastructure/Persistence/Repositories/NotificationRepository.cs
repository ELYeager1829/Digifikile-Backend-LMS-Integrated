using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DigiFikileLms.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly DigiFikileLmsDbContext _context;

    public NotificationRepository(DigiFikileLmsDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Notification>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
        => await _context.Notifications.AsNoTracking().Where(x => x.UserId == userId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Notification>> GetUnreadByUserAsync(int userId, CancellationToken cancellationToken = default)
        => await _context.Notifications.AsNoTracking().Where(x => x.UserId == userId && !x.IsRead).ToListAsync(cancellationToken);

    public async Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default)
        => await _context.Notifications.AsNoTracking().CountAsync(x => x.UserId == userId && !x.IsRead, cancellationToken);

    public async Task<IEnumerable<Notification>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Notifications.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync(notification, cancellationToken);
    }

    public Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Update(notification);
        return Task.CompletedTask;
    }

    public async Task MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _context.Notifications.Where(x => x.UserId == userId && !x.IsRead).ToListAsync(cancellationToken);
        foreach (var notification in notifications)
        {
            notification.MarkAsRead();
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Notifications.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
