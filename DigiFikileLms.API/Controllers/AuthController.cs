using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using DigiFikileLms.Application.DTOs.Auth;
using DigiFikileLms.Application.Features.Auth.Commands;
using DigiFikileLms.Application.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("student/register")]
    [AllowAnonymous]
    public async Task<IActionResult> StudentRegister([FromBody] StudentRegisterRequestDto request)
    {
        var command = new RegisterStudentCommand(
            request.Name,
            request.Surname,
            request.Email,
            request.Password,
            request.StudentNumber,
            request.Phone,
            request.Address);

        try
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return BadRequest(BaseResponse<AuthResponseDto>.Failure("Email or student number already registered"));
        }
    }

    // ================================================================
    // STUDENT LOGIN - Login with Student Number
    // ================================================================
    [HttpPost("student/login")]
    [AllowAnonymous]
    public async Task<IActionResult> StudentLogin([FromBody] StudentLoginRequestDto request)
    {
        var command = new StudentLoginCommand(request.StudentNumber, request.Password);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }

    // ================================================================
    // ADMIN LOGIN - Login with Email
    // ================================================================
    [HttpPost("admin/login")]
    [AllowAnonymous]
    public async Task<IActionResult> AdminLogin([FromBody] AdminLoginRequestDto request)
    {
        var command = new AdminLoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }

    // ================================================================
    // LOGIN STEP 2 (every role) - exchange the 6-digit OTP for the JWT
    // ================================================================
    /// <summary>
    /// Second step of every login: submit the six-digit PIN that was mailed to the account.
    /// A successful call returns the JWT and completes the login.
    /// </summary>
    [HttpPost("otp/verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyLoginOtp([FromBody] VerifyOtpRequestDto request)
    {
        var command = new VerifyLoginOtpCommand(request.Email, request.Otp);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }

    // ================================================================
    // REISSUE THE 6-DIGIT OTP FOR A LOGIN THAT IS ALREADY IN PROGRESS
    // ================================================================
    /// <summary>
    /// Sends a replacement six-digit PIN for a login that is already in progress. The password
    /// step is not repeated.
    /// </summary>
    [HttpPost("otp/resend")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendLoginOtp([FromBody] ResendOtpRequestDto request)
    {
        var command = new ResendLoginOtpCommand(request.Email);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // SYSTEM ADMIN LOGIN - Email + password, then the same 6-digit OTP
    // ================================================================
    /// <summary>
    /// First step of the System Administrator login. Accepts only SystemAdministrator accounts
    /// and mails a six-digit PIN before any JWT is issued.
    /// </summary>
    [HttpPost("system-admin/login")]
    [AllowAnonymous]
    public async Task<IActionResult> SystemAdminLogin([FromBody] AdminLoginRequestDto request)
    {
        var command = new SystemAdminLoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }

    // ================================================================
    // FACILITATOR LOGIN - Email + password, then the same 6-digit OTP
    // ================================================================
    /// <summary>
    /// First step of the Facilitator login. Accepts only Facilitator accounts and mails a
    /// six-digit PIN before any JWT is issued.
    /// </summary>
    [HttpPost("facilitator/login")]
    [AllowAnonymous]
    public async Task<IActionResult> FacilitatorLogin([FromBody] StaffLoginRequestDto request)
    {
        var command = new FacilitatorLoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }

    // ================================================================
    // MODERATOR LOGIN - Email + password, then the same 6-digit OTP
    // ================================================================
    /// <summary>
    /// First step of the Moderator login. Accepts only Moderator accounts and mails a six-digit
    /// PIN before any JWT is issued.
    /// </summary>
    [HttpPost("moderator/login")]
    [AllowAnonymous]
    public async Task<IActionResult> ModeratorLogin([FromBody] StaffLoginRequestDto request)
    {
        var command = new ModeratorLoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }

    // ================================================================
    // TRAINING PROVIDER LOGIN - Email + password, then the 6-digit OTP
    // ================================================================
    /// <summary>
    /// First step of the Training Provider login. Accepts only TrainingProvider accounts and mails
    /// a six-digit PIN before any JWT is issued.
    /// </summary>
    [HttpPost("training-provider/login")]
    [AllowAnonymous]
    public async Task<IActionResult> TrainingProviderLogin([FromBody] StaffLoginRequestDto request)
    {
        var command = new TrainingProviderLoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }



    // ================================================================
    // CURRENT USER / SELF PROFILE - identity always comes from JWT claims
    // ================================================================
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized(BaseResponse<UserDto>.Failure("Authenticated user could not be resolved"));

        var result = await _mediator.Send(new GetCurrentUserQuery(userId));
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPut("me/profile")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrentUserProfile([FromBody] CurrentUserProfileUpdateDto request)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized(BaseResponse<UserDto>.Failure("Authenticated user could not be resolved"));

        var result = await _mediator.Send(new UpdateCurrentUserProfileCommand(
            userId, request.Name, request.Surname, request.Phone, request.Address));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // FORGOT / RESET PASSWORD - recovery OTP is separate from login OTP
    // ================================================================
    [HttpPost("password/forgot")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var result = await _mediator.Send(new ForgotPasswordCommand(request.Email));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("password/reset/verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyPasswordResetOtp([FromBody] VerifyPasswordResetOtpRequestDto request)
    {
        var result = await _mediator.Send(new VerifyPasswordResetOtpCommand(request.Email, request.Otp));
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("password/reset")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var result = await _mediator.Send(new CompletePasswordResetCommand(request.Email, request.ResetToken, request.NewPassword));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // CHANGE PASSWORD - used by administrators holding a temporary password
    // ================================================================
    /// <summary>
    /// Replaces the authenticated account's password. A SETA Administrator who received a
    /// temporary password calls this after the first login.
    /// </summary>
    [HttpPost("password/change")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized(BaseResponse<bool>.Failure("Authenticated user could not be resolved"));

        var command = new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

