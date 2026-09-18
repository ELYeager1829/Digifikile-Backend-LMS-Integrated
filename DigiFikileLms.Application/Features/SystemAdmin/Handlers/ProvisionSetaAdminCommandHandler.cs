using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.SystemAdmin;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.SystemAdmin;

// ================================================================
// PROVISION SETA ADMINISTRATOR HANDLER
// ================================================================

/// <summary>
/// TYPE: ProvisionSetaAdminCommandHandler
/// PURPOSE: Registers a SETA Administrator with system generated sign-in credentials and returns
///          the username plus temporary password so they can be handed to the account holder.
/// LMS ROLE: Supports the SystemAdmin area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - The caller must be an active SystemAdministrator; the id comes from the bearer token.
///   - The clear-text temporary password exists only in the response; the database stores the hash.
///   - The audit log records the provisioning event, never the password.
///
/// NEVER:
///   - Persist or log the temporary password.
///   - Accept a password from the request body; this endpoint is for generated credentials.
/// </summary>
public class ProvisionSetaAdminCommandHandler : IRequestHandler<ProvisionSetaAdminCommand, BaseResponse<SetaAdminCredentialsDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IAdministratorRepository _adminRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITemporaryPasswordGenerator _temporaryPasswordGenerator;
    private readonly ICredentialsEmailService _credentialsEmailService;
    private readonly ISystemLogRepository _logRepository;
    private readonly ISystemAdministratorRepository _systemAdministratorRepository;

    public ProvisionSetaAdminCommandHandler(
        IUserAccountRepository userRepository,
        IAdministratorRepository adminRepository,
        IPasswordHasher passwordHasher,
        ITemporaryPasswordGenerator temporaryPasswordGenerator,
        ICredentialsEmailService credentialsEmailService,
        ISystemLogRepository logRepository,
        ISystemAdministratorRepository systemAdministratorRepository)
    {
        _userRepository = userRepository;
        _adminRepository = adminRepository;
        _passwordHasher = passwordHasher;
        _temporaryPasswordGenerator = temporaryPasswordGenerator;
        _credentialsEmailService = credentialsEmailService;
        _logRepository = logRepository;
        _systemAdministratorRepository = systemAdministratorRepository;
    }

    public async Task<BaseResponse<SetaAdminCredentialsDto>> Handle(
        ProvisionSetaAdminCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Only an active System Administrator may provision administrative accounts.
        var initiator = await _systemAdministratorRepository.GetByUserIdAsync(request.InitiatorUserId, cancellationToken);
        if (initiator == null || !initiator.IsActive)
            return BaseResponse<SetaAdminCredentialsDto>.Failure("Active System Administrator required");

        // 2. Validate the supplied identity before touching the database.
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Surname) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            !System.Net.Mail.MailAddress.TryCreate(request.Email.Trim(), out _))
        {
            return BaseResponse<SetaAdminCredentialsDto>.Failure("Name, surname and a valid email are required");
        }

        if (await _userRepository.ExistsByEmailAsync(request.Email.Trim(), cancellationToken))
            return BaseResponse<SetaAdminCredentialsDto>.Failure("Email already registered");

        // 3. Generate the temporary password that the account holder will receive.
        var temporaryPassword = _temporaryPasswordGenerator.Generate();

        // 4. Create the user account with the generated credentials.
        var user = UserAccount.Create(
            request.Name,
            request.Surname,
            request.Email.Trim(),
            _passwordHasher.HashPassword(temporaryPassword),
            UserRole.SetaAdministrator,
            request.Phone,
            request.Address);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // 5. Create the SETA Administrator profile.
        var administrator = SetaAdministrator.Create(user);
        await _adminRepository.AddAsync(administrator, cancellationToken);
        await _adminRepository.SaveChangesAsync(cancellationToken);

        // 6. Deliver the credentials to the account holder. The account is already committed, so a
        //    mail outage must not fail the provisioning: the failure is reported instead and the
        //    temporary password is returned for manual hand-over.
        var credentialsEmailed = false;
        try
        {
            await _credentialsEmailService.SendTemporaryCredentialsAsync(user, temporaryPassword, cancellationToken);
            credentialsEmailed = true;
        }
        catch (Exception)
        {
            // Reported through the response and the audit log; the created account stays usable.
        }

        // 7. Audit the provisioning (and the mail outcome) without recording the password.
        var log = SystemLog.Create(
            initiator.Id,
            "Create",
            "SETA Administrator",
            user.Id.ToString(),
            $"Provisioned SETA Administrator {user.Email}; credentials e-mailed: {credentialsEmailed}");

        await _logRepository.AddAsync(log, cancellationToken);
        await _logRepository.SaveChangesAsync(cancellationToken);

        // 8. Return the username plus the mail outcome. The clear-text password is only returned
        //    when the mail could not be sent, so it is never exposed unnecessarily.
        return BaseResponse<SetaAdminCredentialsDto>.Success(new SetaAdminCredentialsDto
        {
            UserId = user.Id,
            SetaAdministratorId = administrator.Id,
            Name = user.Name,
            Surname = user.Surname,
            Username = user.Email,
            Email = user.Email,
            TemporaryPassword = credentialsEmailed ? null : temporaryPassword,
            CredentialsEmailed = credentialsEmailed,
            EmailStatus = credentialsEmailed
                ? $"The sign-in credentials were e-mailed to {user.Email}."
                : "The credentials e-mail could not be sent. Hand the temporary password over manually.",
            PasswordIsTemporary = true,
            CreatedAt = user.CreatedAt
        });
    }
}