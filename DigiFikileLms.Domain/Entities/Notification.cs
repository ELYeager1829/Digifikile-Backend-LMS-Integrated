using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Notification : BaseEntity
{
    private Notification() { }

    public static Notification Create(
        UserAccount user,
        string title,
        string message)
    {
        return new Notification
        {
            UserId = user.Id,
            User = user,
            Title = title,
            Message = message,
            DateSent = DateTime.UtcNow,
            IsRead = false
        };
    }

    public int UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public DateTime DateSent { get; private set; }
    public bool IsRead { get; private set; }

    public virtual UserAccount User { get; private set; } = null!;

    public void MarkAsRead()
    {
        IsRead = true;
        UpdateTimestamp();
    }
}