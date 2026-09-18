using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.Features.Users.Queries;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Users.Commands;

public record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<BaseResponse<UserDto>>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, BaseResponse<UserDto>>
{
    public Task<BaseResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken) =>
        Task.FromResult(BaseResponse<UserDto>.Failure("Generic user creation is not enabled through this endpoint yet."));
}

public record UpdateUserCommand(
    int Id,
    string? FirstName = null,
    string? LastName = null,
    string? Phone = null,
    string? Address = null,
    bool? IsActive = null,
    int InitiatorUserId = 0) : IRequest<BaseResponse<UserDto>>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, BaseResponse<UserDto>>
{
    private readonly IUserAccountRepository _users;
    private readonly ISystemAdministratorRepository _systemAdmins;
    private readonly ISystemLogRepository _logs;

    public UpdateUserCommandHandler(
        IUserAccountRepository users,
        ISystemAdministratorRepository systemAdmins,
        ISystemLogRepository logs)
    {
        _users = users;
        _systemAdmins = systemAdmins;
        _logs = logs;
    }

    public async Task<BaseResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetWithDetailsAsync(request.Id, cancellationToken);
        if (user is null) return BaseResponse<UserDto>.Failure("User not found");

        var name = request.FirstName ?? user.Name;
        var surname = request.LastName ?? user.Surname;
        user.UpdateProfile(name, surname, request.Phone ?? user.Phone, request.Address ?? user.Address);

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value) user.Activate(); else user.Deactivate();
        }

        await _users.UpdateAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);
        await AuditAsync(
            request.InitiatorUserId,
            user.Id,
            request.IsActive.HasValue ? (user.IsActive ? "User Reactivated" : "User Deactivated") : "User Updated",
            user.Email,
            cancellationToken);

        return BaseResponse<UserDto>.Success(GetUserByIdQueryHandler.Map(user));
    }

    private async Task AuditAsync(int initiatorUserId, int resourceId, string action, string email, CancellationToken ct)
    {
        if (initiatorUserId <= 0) return;
        var admin = await _systemAdmins.GetByUserIdAsync(initiatorUserId, ct);
        if (admin is null) return;
        await _logs.AddAsync(SystemLog.Create(admin.Id, action, "User", resourceId.ToString(), $"{action}: {email}"), ct);
        await _logs.SaveChangesAsync(ct);
    }
}

public record AssignRoleCommand(int UserId, int RoleId, int InitiatorUserId = 0) : IRequest<BaseResponse<UserDto>>;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, BaseResponse<UserDto>>
{
    private readonly IUserAccountRepository _users;
    private readonly IRoleRepository _roles;
    private readonly ISystemAdministratorRepository _systemAdmins;
    private readonly ISystemLogRepository _logs;

    public AssignRoleCommandHandler(
        IUserAccountRepository users,
        IRoleRepository roles,
        ISystemAdministratorRepository systemAdmins,
        ISystemLogRepository logs)
    {
        _users = users;
        _roles = roles;
        _systemAdmins = systemAdmins;
        _logs = logs;
    }

    public async Task<BaseResponse<UserDto>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetWithDetailsAsync(request.UserId, cancellationToken);
        if (user is null) return BaseResponse<UserDto>.Failure("User not found");

        var role = await _roles.GetWithPermissionsAsync(request.RoleId, cancellationToken);
        if (role is null) return BaseResponse<UserDto>.Failure("Role not found");

        var mappedRole = MapBuiltInRole(role.Name);
        if (mappedRole.HasValue &&
            (mappedRole.Value == DigiFikileLms.Domain.Enums.UserRole.SystemAdministrator || mappedRole.Value == DigiFikileLms.Domain.Enums.UserRole.SetaAdministrator) &&
            user.UserRole != mappedRole.Value)
        {
            return BaseResponse<UserDto>.Failure("Administrative account types must be provisioned through their dedicated workflow.");
        }

        user.AssignRole(role);
        if (mappedRole.HasValue) user.ChangeUserRole(mappedRole.Value);
        await _users.UpdateAsync(user, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);

        if (request.InitiatorUserId > 0)
        {
            var admin = await _systemAdmins.GetByUserIdAsync(request.InitiatorUserId, cancellationToken);
            if (admin is not null)
            {
                await _logs.AddAsync(SystemLog.Create(admin.Id, "Role Assigned", "User", user.Id.ToString(), $"Assigned role {role.Name} to {user.Email}"), cancellationToken);
                await _logs.SaveChangesAsync(cancellationToken);
            }
        }

        return BaseResponse<UserDto>.Success(GetUserByIdQueryHandler.Map(user));
    }
    private static DigiFikileLms.Domain.Enums.UserRole? MapBuiltInRole(string roleName)
    {
        var normalized = new string(roleName.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        return normalized switch
        {
            "student" or "learner" => DigiFikileLms.Domain.Enums.UserRole.Student,
            "facilitator" or "instructor" => DigiFikileLms.Domain.Enums.UserRole.Facilitator,
            "moderator" => DigiFikileLms.Domain.Enums.UserRole.Moderator,
            "trainingprovider" or "provider" => DigiFikileLms.Domain.Enums.UserRole.TrainingProvider,
            "setaadministrator" or "setaadmin" => DigiFikileLms.Domain.Enums.UserRole.SetaAdministrator,
            "systemadministrator" or "systemadmin" => DigiFikileLms.Domain.Enums.UserRole.SystemAdministrator,
            _ => null
        };
    }

}
