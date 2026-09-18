namespace DigiFikileLms.Application.DTOs.SystemLog;

public class SystemLogDto
{
    public int Id { get; set; }
    public int SystemAdministratorId { get; set; }
    public string SystemAdministratorName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string? ResourceId { get; set; }
    public string? Description { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SystemLogFilterDto
{
    public string? Action { get; set; }
    public string? ResourceType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}