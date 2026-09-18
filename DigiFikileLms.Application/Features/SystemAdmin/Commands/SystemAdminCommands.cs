using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.SystemAdmin;
using DigiFikileLms.Application.DTOs.Roles;
using DigiFikileLms.Application.DTOs.Permission;
using DigiFikileLms.Application.DTOs.SystemLog;
using MediatR;

namespace DigiFikileLms.Application.Features.SystemAdmin;

// "As a System Administrator, I want to create SETA Administrator accounts"
public record CreateSetaAdministratorCommand(
    string Name,
    string Surname,
    string Email,
    string Password,
    string? Phone,
    string? Address,
    List<SystemAdminPermissionDto> Permissions,
    int InitiatorUserId
) : IRequest<BaseResponse<SystemAdminDto>>;

// "As a System Administrator, I want to update SETA Administrator permissions"
public record UpdateSetaAdminPermissionsCommand(
    int SetaAdminUserId,
    List<SystemAdminPermissionDto> Permissions
) : IRequest<BaseResponse<bool>>;

// "As a System Administrator, I want to deactivate a SETA Administrator account"
public record DeactivateSetaAdminCommand(
    int SetaAdminUserId,
    int InitiatorUserId = 0
) : IRequest<BaseResponse<bool>>;

public record ActivateSetaAdminCommand(
    int SetaAdminUserId,
    int InitiatorUserId = 0
) : IRequest<BaseResponse<bool>>;

public record UpdateSetaAdminCommand(
    int SetaAdminId,
    string Name,
    string Surname,
    string Email,
    string? Phone,
    string? Address,
    bool? IsActive,
    int InitiatorUserId
) : IRequest<BaseResponse<SystemAdminDto>>;

public record DeleteSetaAdminCommand(
    int SetaAdminId,
    int InitiatorUserId
) : IRequest<BaseResponse<bool>>;


// "As a System Administrator, I want to update my profile"
public record UpdateSystemAdminProfileCommand(
    int SystemAdminId,
    string? Phone,
    string? Address
) : IRequest<BaseResponse<SystemAdminDto>>;


//role management
// "As a System Administrator, I want to create a role"
public record CreateRoleCommand(
    string Name,
    string? Description
) : IRequest<BaseResponse<RoleDto>>;

// "As a System Administrator, I want to update a role"
public record UpdateRoleCommand(
    int RoleId,
    string Name,
    string? Description
) : IRequest<BaseResponse<RoleDto>>;

// "As a System Administrator, I want to delete a role"
public record DeleteRoleCommand(
    int RoleId
) : IRequest<BaseResponse<bool>>;


//permission management

// "As a System Administrator, I want to create a permission"
public record CreatePermissionCommand(
    string Name,
    string? Description,
    string? Category
) : IRequest<BaseResponse<PermissionDto>>;

// "As a System Administrator, I want to delete a permission"
public record DeletePermissionCommand(
    int PermissionId
) : IRequest<BaseResponse<bool>>;


//role-persmission assignment management
// "As a System Administrator, I want to assign permissions to a role"
public record AssignPermissionsToRoleCommand(
    int RoleId,
    List<int> PermissionIds
) : IRequest<BaseResponse<bool>>;

// "As a System Administrator, I want to remove permissions from a role"
public record RemovePermissionsFromRoleCommand(
    int RoleId,
    List<int> PermissionIds
) : IRequest<BaseResponse<bool>>;


//user-role assignment management
// "As a System Administrator, I want to assign roles to users"
public record AssignRolesToUserCommand(
    int UserId,
    List<int> RoleIds
) : IRequest<BaseResponse<bool>>;

// "As a System Administrator, I want to remove roles from users"
public record RemoveRolesFromUserCommand(
    int UserId,
    List<int> RoleIds
) : IRequest<BaseResponse<bool>>;


//profile management


//complaint management
// "As a System Administrator, I want to respond to complaints"
public record RespondToComplaintCommand(
    int ComplaintId,
    string Response,
    string Status // InReview, Resolved, Closed
) : IRequest<BaseResponse<ComplaintDto>>;

public record UpdateComplaintStatusCommand(
    int ComplaintId,
    string Status
) : IRequest<BaseResponse<bool>>;