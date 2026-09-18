using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Moderators;
using MediatR;

namespace DigiFikileLms.Application.Features.Moderators.Commands;

public record CreateModeratorCommand(
    string Name,
    string Surname,
    string Email,
    string Password,
    string? Phone = null,
    string? Address = null,
    string? StaffNumber = null
) : IRequest<BaseResponse<ModeratorDto>>;

public record UpdateModeratorCommand(
    int Id,
    string? Name,
    string? Surname,
    string? Email,
    string? Phone,
    string? Address
) : IRequest<BaseResponse<ModeratorDto>>;

public record DeleteModeratorCommand(int Id) : IRequest<BaseResponse<bool>>;