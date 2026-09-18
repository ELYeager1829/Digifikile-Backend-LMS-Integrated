using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.SetaAdministrators;
using MediatR;

namespace DigiFikileLms.Application.Features.SetaAdministrators.Queries;

public record GetSetaAdministratorByIdQuery(int Id) : IRequest<BaseResponse<SetaAdministratorDto>>;

public record GetSetaAdministratorByUserIdQuery(int UserId) : IRequest<BaseResponse<SetaAdministratorDto>>;

public record GetAllSetaAdministratorsQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<BaseResponse<PagedResponse<SetaAdministratorDto>>>;