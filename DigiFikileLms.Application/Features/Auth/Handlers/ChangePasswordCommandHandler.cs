using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.Features.Auth.Commands;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Auth;

/// <summary>
/// TYPE: ChangePasswordCommandHandler
/// PURPOSE: Replaces the authenticated account's password after re-checking the current one.
/// LMS ROLE: Lets an administrator who received a temporary password replace it with a private
///          one through POST /api/Auth/password/change.
///
/// IMPLEMENTATION GUIDE:
///   - The user id comes from the bearer token, never from the request body.
///   - The current password must be verified before the stored hash is replaced.
///   - The new password is hashed through IPasswordHasher; the plain value is never stored.
/// </summary>
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, BaseResponse<bool>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IUserAccountRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<BaseResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.NewPassword) || request.NewPassword.Length < 8)
            return BaseResponse<bool>.Failure("The new password must be at least 8 characters long");

        if (string.IsNullOrEmpty(request.CurrentPassword))
            return BaseResponse<bool>.Failure("The current password is required");

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return BaseResponse<bool>.Failure("User not found");

        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.Password))
            return BaseResponse<bool>.Failure("The current password is incorrect");

        if (_passwordHasher.VerifyPassword(request.NewPassword, user.Password))
            return BaseResponse<bool>.Failure("The new password must be different from the current password");

        user.UpdatePassword(_passwordHasher.HashPassword(request.NewPassword));
        await _userRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<bool>.Success(true);
    }
}