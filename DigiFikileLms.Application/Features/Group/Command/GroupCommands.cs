using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Group;
using MediatR;

namespace DigiFikileLms.Application.Features.Groups;

public record CreateGroupCommand(
    int setaAdministratorId,
    int? setaProgrammeId,
    string GroupName,
    string? Description
) : IRequest<BaseResponse<GroupDto>>;

public record UpdateGroupCommand(
    int Id,
    int RequestingUserId,
    string GroupName,
    string? Description
) : IRequest<BaseResponse<GroupDto>>;

public record DeleteGroupCommand(
    int Id,
    int RequestingUserId
) : IRequest<BaseResponse<bool>>;

public record AssignStudentsToGroupCommand(
    int GroupId,
    int RequestingUserId,
    List<int> StudentIds
) : IRequest<BaseResponse<bool>>;

public record RemoveStudentFromGroupCommand(
    int GroupId,
    int RequestingUserId,
    int StudentId
) : IRequest<BaseResponse<bool>>;
