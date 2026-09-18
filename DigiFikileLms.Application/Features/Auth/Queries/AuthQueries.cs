using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Auth;


// QUERIES


public record GetCurrentUserQuery(
    int UserId
) : IRequest<BaseResponse<UserDto>>;

public record GetUserPermissionsQuery(
    int UserId
) : IRequest<BaseResponse<List<string>>>;
