namespace DigiFikileLms.Application.DTOs.Group;

public class GroupDto
{
    public int Id { get; set; }
    public int setaAdministratorId { get; set; }
    public string setaAdministratorName { get; set; } = string.Empty;
    public int? setaProgrammeId { get; set; }
    public string? setaProgrammeName { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateGroupDto
{
    public int? setaProgrammeId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateGroupDto
{
    public string GroupName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class GroupEnrollmentDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
}

public class AssignStudentsToGroupDto
{
    public List<int> StudentIds { get; set; } = new();
}