using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Moderators;
using MediatR;

namespace DigiFikileLms.Application.Features.Moderators.Queries;

public record GetModeratorByIdQuery(int Id) : IRequest<BaseResponse<ModeratorDto>>;

public record GetModeratorByUserIdQuery(int UserId) : IRequest<BaseResponse<ModeratorDto>>;

public record GetAllModeratorsQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<BaseResponse<PagedResponse<ModeratorDto>>>;