namespace DigiFikileLms.Application.DTOs.SystemAdmin;

public class SystemLogStatisticsDto
{
    public int TotalLogs { get; set; }
    public int LoginCount { get; set; }
    public int CreateCount { get; set; }
    public int UpdateCount { get; set; }
    public int DeleteCount { get; set; }
    public int ExportCount { get; set; }
    public List<TopUserDto> MostActiveUsers { get; set; } = new();
    public List<DailyLogCountDto> DailyCounts { get; set; } = new();
}

public class TopUserDto
{
    public string UserName { get; set; } = string.Empty;
    public int ActionCount { get; set; }
}

public class DailyLogCountDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}