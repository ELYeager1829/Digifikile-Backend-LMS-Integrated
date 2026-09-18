using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Permission;
using MediatR;

namespace DigiFikileLms.Application.Features.Permissions.Queries;

public record GetPermissionsQuery : IRequest<BaseResponse<IReadOnlyList<PermissionDto>>>;

public record GetPermissionByIdQuery(int Id) : IRequest<BaseResponse<PermissionDto>>;