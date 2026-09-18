using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Auth.Commands;

public record UpdateCurrentUserProfileCommand(
    int UserId,
    string Name,
    string Surname,
    string? Phone,
    string? Address
) : IRequest<BaseResponse<UserDto>>;

public class UpdateCurrentUserProfileCommandHandler
    : IRequestHandler<UpdateCurrentUserProfileCommand, BaseResponse<UserDto>>
{
    private readonly IUserAccountRepository _userRepository;

    public UpdateCurrentUserProfileCommandHandler(IUserAccountRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<UserDto>> Handle(
        UpdateCurrentUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Surname))
            return BaseResponse<UserDto>.Failure("Name and surname are required");

        var user = await _userRepository.GetWithDetailsAsync(request.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<UserDto>.Failure("User not found");

        user.UpdateProfile(
            request.Name.Trim(),
            request.Surname.Trim(),
            string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim());

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<UserDto>.Success(ToDto(user));
    }

    private static UserDto ToDto(DigiFikileLms.Domain.Entities.UserAccount user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        Surname = user.Surname,
        Role = user.UserRole.ToString(),
        RoleId = user.RoleId,
        IsActive = user.IsActive,
        Phone = user.Phone,
        Address = user.Address,
        CreatedAt = user.CreatedAt,
        StudentNumber = user.Student?.StudentNumber,
        Permissions = user.Role?.Permissions?.Select(p => p.Code).ToList() ?? new List<string>()
    };
}
