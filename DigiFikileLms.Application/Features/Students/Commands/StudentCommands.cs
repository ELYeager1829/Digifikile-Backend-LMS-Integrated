using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Students;
using MediatR;

namespace DigiFikileLms.Application.Features.Students;

public record CreateStudentCommand(
    string Name,
    string Surname,
    string Email,
    string Password,
    string? Phone = null,
    string? Address = null,
    string? StudentNumber = null
) : IRequest<BaseResponse<StudentProfileDto>>;

public record UpdateStudentCommand(
    int Id,
    string? Name,
    string? Surname,
    string? Email,
    string? Phone,
    string? Address,
    bool? IsActive
) : IRequest<BaseResponse<StudentProfileDto>>;

public record DeleteStudentCommand(
    int Id
) : IRequest<BaseResponse<bool>>;

public record UpdateStudentProfileCommand(
    int UserId,
    string? Phone,
    string? Address
) : IRequest<BaseResponse<StudentProfileDto>>;