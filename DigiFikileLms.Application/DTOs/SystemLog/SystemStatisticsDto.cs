namespace DigiFikileLms.Application.DTOs.SystemAdmin;

public class SystemStatisticsDto
{
    public int TotalUsers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalFacilitators { get; set; }
    public int TotalAssessors { get; set; }
    public int TotalSetaAdministrators { get; set; }
    public int TotalSystemAdministrators { get; set; }
    public int TotalCourses { get; set; }
    public int TotalGroups { get; set; }
    public int TotalSystemLogs { get; set; }
    public int TotalComplaints { get; set; }
    public int PendingComplaints { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
}