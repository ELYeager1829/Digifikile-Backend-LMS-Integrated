using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.DTOs.SystemAdmin;
using DigiFikileLms.Application.DTOs.Roles;
using DigiFikileLms.Application.DTOs.Permission;
using DigiFikileLms.Application.DTOs.SystemLog;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;
using DigiFikileLms.Application.Interfaces;  //IPasswordHasher
using System.Text;
using MediatR;

namespace DigiFikileLms.Application.Features.SystemAdmin;

// ================================================================
// CREATE SETA ADMINISTRATOR HANDLER
// ================================================================

public class CreateSetaAdministratorCommandHandler : IRequestHandler<CreateSetaAdministratorCommand, BaseResponse<SystemAdminDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IAdministratorRepository _adminRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISystemLogRepository _logRepository;
    private readonly ISystemAdministratorRepository _systemAdministratorRepository;

    public CreateSetaAdministratorCommandHandler(
        IUserAccountRepository userRepository,
        IAdministratorRepository adminRepository,
        IPasswordHasher passwordHasher,
        ISystemLogRepository logRepository,
        ISystemAdministratorRepository systemAdministratorRepository)
    {
        _userRepository = userRepository;
        _adminRepository = adminRepository;
        _passwordHasher = passwordHasher;
        _logRepository = logRepository;
        _systemAdministratorRepository = systemAdministratorRepository;
    }

    public async Task<BaseResponse<SystemAdminDto>> Handle(CreateSetaAdministratorCommand request, CancellationToken cancellationToken)
    {
        var initiator = await _systemAdministratorRepository.GetByUserIdAsync(request.InitiatorUserId, cancellationToken);
        if (initiator == null || !initiator.IsActive)
            return BaseResponse<SystemAdminDto>.Failure("Active System Administrator required");

        // 1. Check if email already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            return BaseResponse<SystemAdminDto>.Failure("Email already registered");

        // 2. Create user account
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = UserAccount.Create(
            request.Name,
            request.Surname,
            request.Email,
            passwordHash,
            UserRole.SetaAdministrator,
            request.Phone,
            request.Address);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // 3. Create administrator profile
        var administrator = SetaAdministrator.Create(user);
        await _adminRepository.AddAsync(administrator, cancellationToken);
        await _adminRepository.SaveChangesAsync(cancellationToken);

        // 4. Log the action
        var log = SystemLog.Create(
            initiator.Id,
            "Create",
            "SETA Administrator",
            user.Id.ToString(),
            $"Created SETA Administrator: {user.Email}");

        await _logRepository.AddAsync(log, cancellationToken);
        await _logRepository.SaveChangesAsync(cancellationToken);

        // 5. Return response
        return BaseResponse<SystemAdminDto>.Success(new SystemAdminDto
        {
            Id = administrator.Id,
            UserId = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Phone = user.Phone,
            IsActive = administrator.IsActive,
            CreatedAt = user.CreatedAt,
            LastLogin = administrator.LastLogin
        });
    }
}

// ================================================================
// GET SYSTEM ADMIN PROFILE HANDLER
// ================================================================

public class GetSystemAdminProfileQueryHandler : IRequestHandler<GetSystemAdminProfileQuery, BaseResponse<SystemAdminDto>>
{
    private readonly ISystemAdministratorRepository _systemAdminRepository;

    public GetSystemAdminProfileQueryHandler(ISystemAdministratorRepository systemAdminRepository)
    {
        _systemAdminRepository = systemAdminRepository;
    }

    public async Task<BaseResponse<SystemAdminDto>> Handle(GetSystemAdminProfileQuery request, CancellationToken cancellationToken)
    {
        var admin = await _systemAdminRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (admin == null)
            return BaseResponse<SystemAdminDto>.Failure("System Administrator not found");

        return BaseResponse<SystemAdminDto>.Success(new SystemAdminDto
        {
            Id = admin.Id,
            UserId = admin.UserId,
            Name = admin.User.Name,
            Surname = admin.User.Surname,
            Email = admin.User.Email,
            Phone = admin.User.Phone,
            IsActive = admin.IsActive,
            CreatedAt = admin.User.CreatedAt,
            LastLogin = admin.LastLogin
        });
    }
}

// ================================================================
// GET SETA ADMINISTRATOR BY ID HANDLER
// ================================================================

public class GetSetaAdministratorByIdQueryHandler : IRequestHandler<GetSetaAdministratorByIdQuery, BaseResponse<SystemAdminDto>>
{
    private readonly IAdministratorRepository _adminRepository;

    public GetSetaAdministratorByIdQueryHandler(IAdministratorRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<BaseResponse<SystemAdminDto>> Handle(GetSetaAdministratorByIdQuery request, CancellationToken cancellationToken)
    {
        var admin = await _adminRepository.GetByIdAsync(request.Id, cancellationToken);
        if (admin == null)
            return BaseResponse<SystemAdminDto>.Failure("SETA Administrator not found");

        return BaseResponse<SystemAdminDto>.Success(new SystemAdminDto
        {
            Id = admin.Id,
            UserId = admin.UserId,
            Name = admin.User.Name,
            Surname = admin.User.Surname,
            Email = admin.User.Email,
            Phone = admin.User.Phone,
            IsActive = admin.IsActive && admin.User.IsActive,
            CreatedAt = admin.User.CreatedAt,
            LastLogin = admin.LastLogin
        });
    }
}

// ================================================================
// GET ALL SETA ADMINISTRATORS HANDLER
// ================================================================

public class GetAllSetaAdministratorsQueryHandler : IRequestHandler<GetAllSetaAdministratorsQuery, BaseResponse<PagedResponse<SystemAdminDto>>>
{
    private readonly IAdministratorRepository _adminRepository;

    public GetAllSetaAdministratorsQueryHandler(IAdministratorRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<BaseResponse<PagedResponse<SystemAdminDto>>> Handle(GetAllSetaAdministratorsQuery request, CancellationToken cancellationToken)
    {
        var admins = await _adminRepository.GetAllAsync(cancellationToken);
        var adminList = admins.ToList();

        var items = adminList.Select(a => new SystemAdminDto
        {
            Id = a.Id,
            UserId = a.UserId,
            Name = a.User.Name,
            Surname = a.User.Surname,
            Email = a.User.Email,
            Phone = a.User.Phone,
            IsActive = a.IsActive,
            CreatedAt = a.CreatedAt,
            LastLogin = a.LastLogin
        }).ToList();

        var response = new PagedResponse<SystemAdminDto>
        {
            Items = items,
            TotalCount = items.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return BaseResponse<PagedResponse<SystemAdminDto>>.Success(response);
    }
}

// ================================================================
// DEACTIVATE SETA ADMIN HANDLER
// ================================================================

public class DeactivateSetaAdminCommandHandler : IRequestHandler<DeactivateSetaAdminCommand, BaseResponse<bool>>
{
    private readonly IAdministratorRepository _adminRepository;
    private readonly IUserAccountRepository _userRepository;
    private readonly ISystemAdministratorRepository _systemAdministratorRepository;
    private readonly ISystemLogRepository _logRepository;

    public DeactivateSetaAdminCommandHandler(
        IAdministratorRepository adminRepository,
        IUserAccountRepository userRepository,
        ISystemAdministratorRepository systemAdministratorRepository,
        ISystemLogRepository logRepository)
    {
        _adminRepository = adminRepository;
        _userRepository = userRepository;
        _systemAdministratorRepository = systemAdministratorRepository;
        _logRepository = logRepository;
    }

    public async Task<BaseResponse<bool>> Handle(DeactivateSetaAdminCommand request, CancellationToken cancellationToken)
    {
        var admin = await _adminRepository.GetByIdAsync(request.SetaAdminUserId, cancellationToken);
        if (admin == null)
            return BaseResponse<bool>.Failure("SETA Administrator not found");

        admin.Deactivate();
        admin.User.Deactivate();
        await _adminRepository.UpdateAsync(admin, cancellationToken);
        await _adminRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(request.InitiatorUserId, admin.UserId, "SETA Administrator Deactivated", admin.User.Email, cancellationToken);

        return BaseResponse<bool>.Success(true);
    }

    private async Task AuditAsync(int initiatorUserId, int resourceId, string action, string email, CancellationToken ct)
    {
        if (initiatorUserId <= 0) return;
        var initiator = await _systemAdministratorRepository.GetByUserIdAsync(initiatorUserId, ct);
        if (initiator is null) return;
        await _logRepository.AddAsync(SystemLog.Create(initiator.Id, action, "SETA Administrator", resourceId.ToString(), $"{action}: {email}"), ct);
        await _logRepository.SaveChangesAsync(ct);
    }
}

public class ActivateSetaAdminCommandHandler : IRequestHandler<ActivateSetaAdminCommand, BaseResponse<bool>>
{
    private readonly IAdministratorRepository _adminRepository;
    private readonly ISystemAdministratorRepository _systemAdministratorRepository;
    private readonly ISystemLogRepository _logRepository;

    public ActivateSetaAdminCommandHandler(
        IAdministratorRepository adminRepository,
        ISystemAdministratorRepository systemAdministratorRepository,
        ISystemLogRepository logRepository)
    {
        _adminRepository = adminRepository;
        _systemAdministratorRepository = systemAdministratorRepository;
        _logRepository = logRepository;
    }

    public async Task<BaseResponse<bool>> Handle(ActivateSetaAdminCommand request, CancellationToken cancellationToken)
    {
        var admin = await _adminRepository.GetByIdAsync(request.SetaAdminUserId, cancellationToken);
        if (admin == null)
            return BaseResponse<bool>.Failure("SETA Administrator not found");

        admin.Activate();
        admin.User.Activate();
        await _adminRepository.UpdateAsync(admin, cancellationToken);
        await _adminRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(request.InitiatorUserId, admin.UserId, "SETA Administrator Reactivated", admin.User.Email, cancellationToken);

        return BaseResponse<bool>.Success(true);
    }

    private async Task AuditAsync(int initiatorUserId, int resourceId, string action, string email, CancellationToken ct)
    {
        if (initiatorUserId <= 0) return;
        var initiator = await _systemAdministratorRepository.GetByUserIdAsync(initiatorUserId, ct);
        if (initiator is null) return;
        await _logRepository.AddAsync(SystemLog.Create(initiator.Id, action, "SETA Administrator", resourceId.ToString(), $"{action}: {email}"), ct);
        await _logRepository.SaveChangesAsync(ct);
    }
}

public class UpdateSetaAdminCommandHandler : IRequestHandler<UpdateSetaAdminCommand, BaseResponse<SystemAdminDto>>
{
    private readonly IAdministratorRepository _adminRepository;
    private readonly IUserAccountRepository _userRepository;
    private readonly ISystemAdministratorRepository _systemAdministratorRepository;
    private readonly ISystemLogRepository _logRepository;

    public UpdateSetaAdminCommandHandler(
        IAdministratorRepository adminRepository,
        IUserAccountRepository userRepository,
        ISystemAdministratorRepository systemAdministratorRepository,
        ISystemLogRepository logRepository)
    {
        _adminRepository = adminRepository;
        _userRepository = userRepository;
        _systemAdministratorRepository = systemAdministratorRepository;
        _logRepository = logRepository;
    }

    public async Task<BaseResponse<SystemAdminDto>> Handle(UpdateSetaAdminCommand request, CancellationToken cancellationToken)
    {
        var admin = await _adminRepository.GetByIdAsync(request.SetaAdminId, cancellationToken);
        if (admin == null)
            return BaseResponse<SystemAdminDto>.Failure("SETA Administrator not found");

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Surname) ||
            string.IsNullOrWhiteSpace(request.Email) || !System.Net.Mail.MailAddress.TryCreate(request.Email.Trim(), out _))
            return BaseResponse<SystemAdminDto>.Failure("Name, surname and a valid email are required");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (!string.Equals(admin.User.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase) &&
            await _userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken))
            return BaseResponse<SystemAdminDto>.Failure("Email already registered");

        admin.User.UpdateProfile(request.Name.Trim(), request.Surname.Trim(), request.Phone, request.Address);
        admin.User.UpdateEmail(normalizedEmail);

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
            {
                admin.Activate();
                admin.User.Activate();
            }
            else
            {
                admin.Deactivate();
                admin.User.Deactivate();
            }
        }

        await _adminRepository.UpdateAsync(admin, cancellationToken);
        await _adminRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(request.InitiatorUserId, admin.UserId, "SETA Administrator Updated", admin.User.Email, cancellationToken);

        return BaseResponse<SystemAdminDto>.Success(ToDto(admin));
    }

    private static SystemAdminDto ToDto(SetaAdministrator admin) => new()
    {
        Id = admin.Id,
        UserId = admin.UserId,
        Name = admin.User.Name,
        Surname = admin.User.Surname,
        Email = admin.User.Email,
        Phone = admin.User.Phone,
        IsActive = admin.IsActive && admin.User.IsActive,
        CreatedAt = admin.User.CreatedAt,
        LastLogin = admin.LastLogin
    };

    private async Task AuditAsync(int initiatorUserId, int resourceId, string action, string email, CancellationToken ct)
    {
        if (initiatorUserId <= 0) return;
        var initiator = await _systemAdministratorRepository.GetByUserIdAsync(initiatorUserId, ct);
        if (initiator is null) return;
        await _logRepository.AddAsync(SystemLog.Create(initiator.Id, action, "SETA Administrator", resourceId.ToString(), $"{action}: {email}"), ct);
        await _logRepository.SaveChangesAsync(ct);
    }
}

public class DeleteSetaAdminCommandHandler : IRequestHandler<DeleteSetaAdminCommand, BaseResponse<bool>>
{
    private readonly IAdministratorRepository _adminRepository;
    private readonly ISystemAdministratorRepository _systemAdministratorRepository;
    private readonly ISystemLogRepository _logRepository;

    public DeleteSetaAdminCommandHandler(
        IAdministratorRepository adminRepository,
        ISystemAdministratorRepository systemAdministratorRepository,
        ISystemLogRepository logRepository)
    {
        _adminRepository = adminRepository;
        _systemAdministratorRepository = systemAdministratorRepository;
        _logRepository = logRepository;
    }

    public async Task<BaseResponse<bool>> Handle(DeleteSetaAdminCommand request, CancellationToken cancellationToken)
    {
        var admin = await _adminRepository.GetByIdAsync(request.SetaAdminId, cancellationToken);
        if (admin == null)
            return BaseResponse<bool>.Failure("SETA Administrator not found");

        // Use a safe soft-delete/decommission for first production integration: keep audit/history intact
        // and block future authentication instead of physically deleting relational data.
        admin.Deactivate();
        admin.User.Deactivate();
        await _adminRepository.UpdateAsync(admin, cancellationToken);
        await _adminRepository.SaveChangesAsync(cancellationToken);

        await AuditAsync(request.InitiatorUserId, admin.UserId, "SETA Administrator Removed", admin.User.Email, cancellationToken);
        return BaseResponse<bool>.Success(true);
    }

    private async Task AuditAsync(int initiatorUserId, int resourceId, string action, string email, CancellationToken ct)
    {
        if (initiatorUserId <= 0) return;
        var initiator = await _systemAdministratorRepository.GetByUserIdAsync(initiatorUserId, ct);
        if (initiator is null) return;
        await _logRepository.AddAsync(SystemLog.Create(initiator.Id, action, "SETA Administrator", resourceId.ToString(), $"{action}: {email}"), ct);
        await _logRepository.SaveChangesAsync(ct);
    }
}

// ================================================================
// 2. CREATE ROLE
// ================================================================

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, BaseResponse<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public CreateRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<BaseResponse<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await _roleRepository.ExistsByNameAsync(request.Name, cancellationToken))
            return BaseResponse<RoleDto>.Failure("Role already exists");

        var role = Role.Create(request.Name, request.Description);
        await _roleRepository.AddAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<RoleDto>.Success(new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt
        });
    }
}

// ================================================================
// 3. CREATE PERMISSION
// ================================================================

public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, BaseResponse<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;

    public CreatePermissionCommandHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<BaseResponse<PermissionDto>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name))
            return BaseResponse<PermissionDto>.Failure("Permission name is required");

        if (await _permissionRepository.ExistsByNameAsync(request.Name, cancellationToken))
            return BaseResponse<PermissionDto>.Failure("Permission already exists");

        var permission = Permission.Create(request.Name, request.Description, request.Category);
        await _permissionRepository.AddAsync(permission, cancellationToken);
        await _permissionRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<PermissionDto>.Success(new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            Category = permission.Category,
            CreatedAt = permission.CreatedAt
        });
    }
}

// ================================================================
// 4. ASSIGN PERMISSIONS TO ROLE
// ================================================================

public class AssignPermissionsToRoleCommandHandler : IRequestHandler<AssignPermissionsToRoleCommand, BaseResponse<bool>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public AssignPermissionsToRoleCommandHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<BaseResponse<bool>> Handle(AssignPermissionsToRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
            return BaseResponse<bool>.Failure("Role not found");

        foreach (var permissionId in request.PermissionIds)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId, cancellationToken);
            if (permission != null)
                role.AddPermission(permission);
        }

        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<bool>.Success(true);
    }
}

// ================================================================
// 5. GET ALL ROLES
// ================================================================

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, BaseResponse<List<RoleDto>>>
{
    private readonly IRoleRepository _roleRepository;

    public GetAllRolesQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<BaseResponse<List<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);

        var result = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            PermissionCount = r.RolePermissions.Count,
            CreatedAt = r.CreatedAt,
            Permissions = r.RolePermissions.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
                Description = rp.Permission.Description,
                Category = rp.Permission.Category
            }).ToList()
        }).ToList();

        return BaseResponse<List<RoleDto>>.Success(result);
    }
}

// ================================================================
// 6. GET ALL PERMISSIONS
// ================================================================

public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, BaseResponse<List<PermissionDto>>>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetAllPermissionsQueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<BaseResponse<List<PermissionDto>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);

        var result = permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            CreatedAt = p.CreatedAt
        }).ToList();

        return BaseResponse<List<PermissionDto>>.Success(result);
    }
}

// ================================================================
// 7. GET SYSTEM LOGS
// ================================================================

public class GetAllSystemLogsQueryHandler : IRequestHandler<GetAllSystemLogsQuery, BaseResponse<PagedResponse<SystemLogDto>>>
{
    private readonly ISystemLogRepository _logRepository;

    public GetAllSystemLogsQueryHandler(ISystemLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<BaseResponse<PagedResponse<SystemLogDto>>> Handle(GetAllSystemLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _logRepository.GetAllAsync(cancellationToken);
        var logList = logs.ToList();

        var items = logList.Select(l => new SystemLogDto
        {
            Id = l.Id,
            SystemAdministratorId = l.SystemAdministratorId,
            SystemAdministratorName = $"{l.SystemAdministrator.User.Name} {l.SystemAdministrator.User.Surname}",
            Action = l.Action,
            ResourceType = l.ResourceType,
            ResourceId = l.ResourceId,
            Description = l.Description,
            IpAddress = l.IpAddress,
            CreatedAt = l.CreatedAt
        }).ToList();

        var response = new PagedResponse<SystemLogDto>
        {
            Items = items,
            TotalCount = items.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return BaseResponse<PagedResponse<SystemLogDto>>.Success(response);
    }
}

// ================================================================
// 8. EXPORT SYSTEM LOGS
// ================================================================

public class ExportSystemLogsQueryHandler : IRequestHandler<ExportSystemLogsQuery, BaseResponse<FileDownloadDto>>
{
    private readonly ISystemLogRepository _logRepository;

    public ExportSystemLogsQueryHandler(ISystemLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<BaseResponse<FileDownloadDto>> Handle(ExportSystemLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _logRepository.SearchAsync(
            request.Filter.Action,
            request.Filter.ResourceType,
            request.Filter.StartDate,
            request.Filter.EndDate,
            cancellationToken);

        var csv = new StringBuilder();
        csv.AppendLine("Id,Administrator,Action,ResourceType,ResourceId,Description,IpAddress,CreatedAt");

        foreach (var log in logs)
        {
            csv.AppendLine($"{log.Id}," +
                $"{log.SystemAdministrator.User.Name} {log.SystemAdministrator.User.Surname}," +
                $"{log.Action}," +
                $"{log.ResourceType}," +
                $"{log.ResourceId}," +
                $"\"{log.Description}\"," +
                $"{log.IpAddress}," +
                $"{log.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        }

        var fileBytes = Encoding.UTF8.GetBytes(csv.ToString());

        return BaseResponse<FileDownloadDto>.Success(new FileDownloadDto
        {
            FileContent = fileBytes,
            FileName = $"SystemLogs_{DateTime.UtcNow:yyyyMMddHHmmss}.csv",
            ContentType = "text/csv"
        });
    }
}

// ================================================================
// 9. GET SYSTEM STATISTICS
// ================================================================

public class GetSystemStatisticsQueryHandler : IRequestHandler<GetSystemStatisticsQuery, BaseResponse<SystemStatisticsDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ISystemLogRepository _logRepository;
    private readonly IAdministratorRepository _adminRepository;

    public GetSystemStatisticsQueryHandler(
        IUserAccountRepository userRepository,
        ICourseRepository courseRepository,
        IGroupRepository groupRepository,
        ISystemLogRepository logRepository,
        IAdministratorRepository adminRepository)
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _groupRepository = groupRepository;
        _logRepository = logRepository;
        _adminRepository = adminRepository;
    }

    public async Task<BaseResponse<SystemStatisticsDto>> Handle(GetSystemStatisticsQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var courses = await _courseRepository.GetAllAsync(cancellationToken);
        var groups = await _groupRepository.GetAllAsync(cancellationToken);
        var logs = await _logRepository.GetAllAsync(cancellationToken);
        var admins = await _adminRepository.GetAllAsync(cancellationToken);

        var userList = users.ToList();

        return BaseResponse<SystemStatisticsDto>.Success(new SystemStatisticsDto
        {
            TotalUsers = userList.Count,
            TotalStudents = userList.Count(u => u.UserRole == UserRole.Student),
            TotalFacilitators = userList.Count(u => u.UserRole == UserRole.Facilitator),
            
            TotalSetaAdministrators = admins.Count(),
            TotalCourses = courses.Count(),
            TotalGroups = groups.Count(),
            TotalSystemLogs = logs.Count(),
            ActiveUsers = userList.Count(u => u.IsActive),
            InactiveUsers = userList.Count(u => !u.IsActive)
        });
    }
}
