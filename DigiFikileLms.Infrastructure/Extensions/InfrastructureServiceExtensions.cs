using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Infrastructure.Persistence.Context;
using DigiFikileLms.Infrastructure.Persistence.Repositories;
using DigiFikileLms.Infrastructure.Services;

namespace DigiFikileLms.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ✅ Use DigiFikileLmsDbContext
        services.AddDbContext<DigiFikileLmsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DigiFikileLmsDb"),
                b => b.MigrationsAssembly(typeof(DigiFikileLmsDbContext).Assembly.FullName)));

        // Short-lived OTP/reset challenges are kept in process memory for the current deployment.
        services.AddMemoryCache();
        services.AddScoped<EmailService>();

        // Register Application technical service interfaces with their implementations
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<ITokenService, TokenService>();

        // Two-step login OTP delivery uses the existing SMTP-backed EmailService.
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IPasswordResetService, PasswordResetService>();

        // System Administrator provisioning of temporary credentials for SETA administrators.
        services.AddSingleton<ITemporaryPasswordGenerator, TemporaryPasswordGenerator>();

        // Mails temporary credentials and the setup/login link to the new SETA administrator.
        services.AddScoped<ICredentialsEmailService, CredentialsEmailService>();

        // Register Repositories
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserAccountRepository, UserAccountRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IFacilitatorRepository, FacilitatorRepository>();
        services.AddScoped<IModeratorRepository, ModeratorRepository>();
        services.AddScoped<ISetaAdministratorRepository, SetaAdministratorRepository>();
        services.AddScoped<ITrainingProviderRepository, TrainingProviderRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ISETAProgrammeRepository, SETAProgrammeRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();
        services.AddScoped<IResultRepository, ResultRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<ICertificateRepository, CertificateRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        services.AddScoped<ISystemAdministratorRepository, SystemAdministratorRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupEnrollmentRepository, GroupEnrollmentRepository>();
        services.AddScoped<ISystemLogRepository, SystemLogRepository>();
        services.AddScoped<IComplaintRepository, ComplaintRepository>();

        services.AddScoped<IAdministratorRepository, AdministratorRepository>();

        return services;
    }
}

