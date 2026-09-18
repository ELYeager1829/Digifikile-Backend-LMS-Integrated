using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class TrainingProvider : BaseEntity
{
    private readonly List<Department> _departments = new();
    private readonly List<Report> _reports = new();

    private TrainingProvider() { }

    public static TrainingProvider Create(UserAccount user, string companyName)
    {
        return new TrainingProvider
        {
            UserId = user.Id,
            User = user,
            CompanyName = companyName
        };
    }

    public int UserId { get; private set; }
    public string CompanyName { get; private set; } = string.Empty;

    public virtual UserAccount User { get; private set; } = null!;
    public IReadOnlyCollection<Department> Departments => _departments.AsReadOnly();
    public IReadOnlyCollection<Report> Reports => _reports.AsReadOnly();

    public void AddDepartment(Department department)
    {
        _departments.Add(department);
        UpdateTimestamp();
    }
}