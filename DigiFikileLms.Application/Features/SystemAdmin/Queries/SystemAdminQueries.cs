using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.SystemAdmin;
using DigiFikileLms.Application.DTOs.SystemLog;
using DigiFikileLms.Application.DTOs.Roles;
using DigiFikileLms.Application.DTOs.Permission;
using DigiFikileLms.Application.DTOs;
using MediatR;

namespace DigiFikileLms.Application.Features.SystemAdmin;

// "As a System Administrator, I want to view my profile"
public record GetSystemAdminProfileQuery(
    int UserId
) : IRequest<BaseResponse<SystemAdminDto>>;

// "As a System Administrator, I want to view all SETA Administrators"
public record GetAllSetaAdministratorsQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<BaseResponse<PagedResponse<SystemAdminDto>>>;

// "As a System Administrator, I want to view a specific SETA Administrator"
public record GetSetaAdministratorByIdQuery(
    int Id
) : IRequest<BaseResponse<SystemAdminDto>>;


//role management
// "As a System Administrator, I want to view all roles"
public record GetAllRolesQuery() : IRequest<BaseResponse<List<RoleDto>>>;

// "As a System Administrator, I want to view a specific role"
public record GetRoleByIdQuery(
    int RoleId
) : IRequest<BaseResponse<RoleDto>>;


//permission management
// "As a System Administrator, I want to view all permissions"
public record GetAllPermissionsQuery() : IRequest<BaseResponse<List<PermissionDto>>>;

// "As a System Administrator, I want to view a specific permission"
public record GetPermissionByIdQuery(
    int PermissionId
) : IRequest<BaseResponse<PermissionDto>>;


//system logs
// "As a System Administrator, I want to view all system logs"
public record GetAllSystemLogsQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<BaseResponse<PagedResponse<SystemLogDto>>>;

// "As a System Administrator, I want to search/filter system logs"
public record SearchSystemLogsQuery(
    SystemLogFilterDto Filter
) : IRequest<BaseResponse<PagedResponse<SystemLogDto>>>;

// "As a System Administrator, I want to view a specific log"
public record GetSystemLogByIdQuery(
    int Id
) : IRequest<BaseResponse<SystemLogDto>>;

// "As a System Administrator, I want to export system logs"
public record ExportSystemLogsQuery(
    SystemLogFilterDto Filter
) : IRequest<BaseResponse<FileDownloadDto>>;

// "As a System Administrator, I want to view log statistics"
public record GetSystemLogStatisticsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<BaseResponse<SystemLogStatisticsDto>>;


//complaints management
// "As a System Administrator, I want to view all complaints"
public record GetAllComplaintsQuery(
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<BaseResponse<PagedResponse<ComplaintDto>>>;

// "As a System Administrator, I want to view a specific complaint"
public record GetComplaintByIdQuery(
    int ComplaintId
) : IRequest<BaseResponse<ComplaintDto>>;


//system settings management
// "As a System Administrator, I want to view system statistics"
public record GetSystemStatisticsQuery() : IRequest<BaseResponse<SystemStatisticsDto>>;
