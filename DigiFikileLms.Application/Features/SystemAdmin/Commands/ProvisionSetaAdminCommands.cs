using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.SystemAdmin;
using MediatR;

namespace DigiFikileLms.Application.Features.SystemAdmin;

// "As a System Administrator, I want to register a SETA Administrator so that the new
//  account holder receives a username and a temporary password that can be changed later"
public record ProvisionSetaAdminCommand(
    string Name,
    string Surname,
    string Email,
    string? Phone,
    string? Address,
    List<SystemAdminPermissionDto> Permissions,
    int InitiatorUserId
) : IRequest<BaseResponse<SetaAdminCredentialsDto>>;