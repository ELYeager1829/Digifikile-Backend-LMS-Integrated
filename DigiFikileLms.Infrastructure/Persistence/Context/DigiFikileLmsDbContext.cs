using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Common;
using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using DomainModule = DigiFikileLms.Domain.Entities.Module;

namespace DigiFikileLms.Infrastructure.Persistence.Context;

public class DigiFikileLmsDbContext : DbContext, IApplicationDbContext
{
    public DigiFikileLmsDbContext(DbContextOptions<DigiFikileLmsDbContext> options) : base(options)
    {
    }

    public DbSet<SystemAdministrator> SystemAdministrators => Set<SystemAdministrator>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Facilitator> Facilitators => Set<Facilitator>();
    public DbSet<Moderator> Moderators => Set<Moderator>();
    public DbSet<SetaAdministrator> SetaAdministrators => Set<SetaAdministrator>();
    public DbSet<TrainingProvider> TrainingProviders => Set<TrainingProvider>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<SETAProgramme> SETAProgrammes => Set<SETAProgramme>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<DomainModule> Modules => Set<DomainModule>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Progress> ProgressRecords => Set<Progress>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Complaint> Complaints => Set<Complaint>();

    //and the one at the top 
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupEnrollment> GroupEnrollments { get; set; }
    public DbSet<SystemLog> SystemLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = utcNow;

            if (entry.State is EntityState.Added or EntityState.Modified)
                entry.Entity.UpdatedAt = utcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
