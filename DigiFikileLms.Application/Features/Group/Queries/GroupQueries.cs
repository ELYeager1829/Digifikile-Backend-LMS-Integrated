using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Group;
using MediatR;

namespace DigiFikileLms.Application.Features.Groups;

public record GetGroupsByAdministratorQuery(
    int AdministratorId
) : IRequest<BaseResponse<List<GroupDto>>>;

public record GetAllGroupsQuery(
    int RequestingUserId,
    int Page = 1,
    int PageSize = 20
) : IRequest<BaseResponse<PagedResponse<GroupDto>>>;

public record GetGroupByIdQuery(
    int Id,
    int RequestingUserId
) : IRequest<BaseResponse<GroupDto>>;

public record GetGroupStudentsQuery(
    int GroupId,
    int RequestingUserId
) : IRequest<BaseResponse<List<GroupEnrollmentDto>>>;
