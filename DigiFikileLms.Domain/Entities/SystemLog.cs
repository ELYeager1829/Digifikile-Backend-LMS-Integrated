using DigiFikileLms.Domain.Common;
using DigiFikileLms.Domain.Enums;

namespace DigiFikileLms.Domain.Entities;

public class SystemLog : BaseEntity
{
    private SystemLog() { }

    public static SystemLog Create(
        int systemAdministratorId,
        string action,
        string resourceType,
        string? resourceId = null,
        string? description = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        return new SystemLog
        {
            SystemAdministratorId = systemAdministratorId,
            Action = action,
            ResourceType = resourceType,
            ResourceId = resourceId,
            Description = description,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow
        };
    }

    public int SystemAdministratorId { get; private set; }
    public string Action { get; private set; } = string.Empty; // Create, Update, Delete, Login, Export
    public string ResourceType { get; private set; } = string.Empty; // User, Course, Group, etc.
    public string? ResourceId { get; private set; }
    public string? Description { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    // Navigation Properties
    public virtual SystemAdministrator SystemAdministrator { get; private set; } = null!;
}