using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Permission;
using DigiFikileLms.Application.DTOs.Roles;
using DigiFikileLms.Application.Features.Roles.Commands;
using DigiFikileLms.Application.Features.Roles.Queries;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Roles;

/// <summary>
/// Creates a role via IRoleRepository. Rejects duplicate role names.
/// </summary>
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
            return BaseResponse<RoleDto>.Failure("Role name already exists");

        var role = Role.Create(request.Name, request.Description);
        await _roleRepository.AddAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<RoleDto>.Success(new RoleDto(role.Id, role.Name, role.Description));
    }
}

/// <summary>
/// Updates the name/description of an existing role.
/// </summary>
public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, BaseResponse<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public UpdateRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<BaseResponse<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role == null)
            return BaseResponse<RoleDto>.Failure("Role not found");

        role.UpdateDetails(request.Name, request.Description);
        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<RoleDto>.Success(new RoleDto(role.Id, role.Name, role.Description));
    }
}

/// <summary>
/// Deletes a role by id.
/// </summary>
public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, BaseResponse<bool>>
{
    private readonly IRoleRepository _roleRepository;

    public DeleteRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<BaseResponse<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role == null)
            return BaseResponse<bool>.Failure("Role not found");

        await _roleRepository.DeleteAsync(request.Id, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<bool>.Success(true);
    }
}
/// <summary>
/// Assigns an existing permission to a role.
/// </summary>
public class AssignPermissionCommandHandler : IRequestHandler<AssignPermissionCommand, BaseResponse<RoleDetailDto>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ISystemAdministratorRepository _systemAdministrators;
    private readonly ISystemLogRepository _logs;

    public AssignPermissionCommandHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        ISystemAdministratorRepository systemAdministrators,
        ISystemLogRepository logs)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _systemAdministrators = systemAdministrators;
        _logs = logs;
    }

    public async Task<BaseResponse<RoleDetailDto>> Handle(AssignPermissionCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetWithPermissionsAsync(request.RoleId, cancellationToken);
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);

        if (role == null)
            return BaseResponse<RoleDetailDto>.Failure("Role not found");
        if (permission == null)
            return BaseResponse<RoleDetailDto>.Failure("Permission not found");

        role.AddPermission(permission);
        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        if (request.InitiatorUserId > 0)
        {
            var administrator = await _systemAdministrators.GetByUserIdAsync(request.InitiatorUserId, cancellationToken);
            if (administrator is not null)
            {
                await _logs.AddAsync(SystemLog.Create(
                    administrator.Id,
                    "Permission Assigned",
                    "Role",
                    role.Id.ToString(),
                    $"Assigned permission {permission.Code} to role {role.Name}"), cancellationToken);
                await _logs.SaveChangesAsync(cancellationToken);
            }
        }

        var reloaded = await _roleRepository.GetWithPermissionsAsync(role.Id, cancellationToken);
        var permissionDtos = reloaded?.Permissions
            .Select(p => new PermissionDto(p.Id, p.Code, p.Name, p.Description))
            .ToList() ?? new List<PermissionDto>();

        return BaseResponse<RoleDetailDto>.Success(new RoleDetailDto(
            role.Id,
            role.Name,
            role.Description,
            permissionDtos));
    }
}

/// <summary>
/// Returns all roles.
/// </summary>
public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, BaseResponse<IReadOnlyList<RoleDto>>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolesQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<BaseResponse<IReadOnlyList<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        var dtos = roles.Select(r => new RoleDto(r.Id, r.Name, r.Description)).ToList();
        return BaseResponse<IReadOnlyList<RoleDto>>.Success(dtos);
    }
}
/// <summary>
/// Returns a single role by id including its permissions.
/// </summary>
public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, BaseResponse<RoleDetailDto>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRoleByIdQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<BaseResponse<RoleDetailDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetWithPermissionsAsync(request.Id, cancellationToken);
        if (role == null)
            return BaseResponse<RoleDetailDto>.Failure("Role not found");

        var permissionDtos = role.Permissions
            .Select(p => new PermissionDto(p.Id, p.Code, p.Name, p.Description))
            .ToList();

        return BaseResponse<RoleDetailDto>.Success(new RoleDetailDto(
            role.Id,
            role.Name,
            role.Description,
            permissionDtos));
    }
}

/// <summary>
/// Returns the permissions assigned to a role.
/// </summary>
public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, BaseResponse<IReadOnlyList<PermissionDto>>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolePermissionsQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<BaseResponse<IReadOnlyList<PermissionDto>>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetWithPermissionsAsync(request.RoleId, cancellationToken);
        if (role == null)
            return BaseResponse<IReadOnlyList<PermissionDto>>.Failure("Role not found");

        var dtos = role.Permissions
            .Select(p => new PermissionDto(p.Id, p.Code, p.Name, p.Description))
            .ToList();

        return BaseResponse<IReadOnlyList<PermissionDto>>.Success(dtos);
    }
}

public class RemovePermissionCommandHandler : IRequestHandler<RemovePermissionCommand, BaseResponse<RoleDetailDto>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ISystemAdministratorRepository _systemAdministrators;
    private readonly ISystemLogRepository _logs;

    public RemovePermissionCommandHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        ISystemAdministratorRepository systemAdministrators,
        ISystemLogRepository logs)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _systemAdministrators = systemAdministrators;
        _logs = logs;
    }

    public async Task<BaseResponse<RoleDetailDto>> Handle(RemovePermissionCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetWithPermissionsAsync(request.RoleId, cancellationToken);
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (role is null) return BaseResponse<RoleDetailDto>.Failure("Role not found");
        if (permission is null) return BaseResponse<RoleDetailDto>.Failure("Permission not found");

        role.RemovePermission(permission);
        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        if (request.InitiatorUserId > 0)
        {
            var administrator = await _systemAdministrators.GetByUserIdAsync(request.InitiatorUserId, cancellationToken);
            if (administrator is not null)
            {
                await _logs.AddAsync(SystemLog.Create(
                    administrator.Id,
                    "Permission Removed",
                    "Role",
                    role.Id.ToString(),
                    $"Removed permission {permission.Code} from role {role.Name}"), cancellationToken);
                await _logs.SaveChangesAsync(cancellationToken);
            }
        }

        var reloaded = await _roleRepository.GetWithPermissionsAsync(role.Id, cancellationToken);
        var permissions = reloaded?.Permissions.Select(p => new PermissionDto(p.Id, p.Code, p.Name, p.Description)).ToList() ?? new List<PermissionDto>();
        return BaseResponse<RoleDetailDto>.Success(new RoleDetailDto(role.Id, role.Name, role.Description, permissions));
    }
}
