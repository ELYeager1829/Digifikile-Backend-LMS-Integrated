using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Permission;
using MediatR;

namespace DigiFikileLms.Application.Features.Permissions.Commands;

public record CreatePermissionCommand(
    string Code,
    string Name,
    string? Description
) : IRequest<BaseResponse<PermissionDto>>;

public record UpdatePermissionCommand(
    int Id,
    string Code,
    string Name,
    string? Description
) : IRequest<BaseResponse<PermissionDto>>;

public record DeletePermissionCommand(int Id) : IRequest<BaseResponse<bool>>;