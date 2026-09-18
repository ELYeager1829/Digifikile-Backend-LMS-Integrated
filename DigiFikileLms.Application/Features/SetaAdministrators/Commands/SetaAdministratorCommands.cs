using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.SetaAdministrators;
using MediatR;

namespace DigiFikileLms.Application.Features.SetaAdministrators.Commands;

public record CreateSetaAdministratorCommand(
    string Name,
    string Surname,
    string Email,
    string Password,
    string? Phone = null,
    string? Address = null
) : IRequest<BaseResponse<SetaAdministratorDto>>;

public record UpdateSetaAdministratorCommand(
    int Id,
    string? Name,
    string? Surname,
    string? Email,
    string? Phone,
    string? Address,
    bool? IsActive
) : IRequest<BaseResponse<SetaAdministratorDto>>;

public record DeleteSetaAdministratorCommand(int Id) : IRequest<BaseResponse<bool>>;