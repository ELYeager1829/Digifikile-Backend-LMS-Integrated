# Two-step login (6-digit OTP) and SETA administrator provisioning

This change adds the endpoint surface for two requirements:

1. every account must submit a six-digit PIN (OTP) after the password step of a login;
2. a System Administrator can register a SETA Administrator account whose username and temporary
   password are handed to the account holder and can be replaced afterwards.

Only the endpoints, their contracts, and the seam the OTP/E-mail owner implements are in place. The
6-digit PIN generation, its storage, and the EmailJS delivery are intentionally left as TODOs.

## 1. Login is now a two-step handshake

| Step | Endpoint | Result |
|---|---|---|
| 1 | `POST /api/Auth/student/login` | Credentials verified, PIN mailed, `otpRequired = true`, `token` empty |
| 1 | `POST /api/Auth/admin/login` | Same handshake for SetaAdministrator accounts |
| 1 | `POST /api/Auth/system-admin/login` | Same handshake for SystemAdministrator accounts (new endpoint) |
| 1 | `POST /api/Auth/facilitator/login` | Same handshake for Facilitator accounts (new endpoint) |
| 1 | `POST /api/Auth/moderator/login` | Same handshake for Moderator accounts (new endpoint) |
| 1 | `POST /api/Auth/training-provider/login` | Same handshake for TrainingProvider accounts (new endpoint) |
| 2 | `POST /api/Auth/otp/verify` | `{ email, otp }` returns the JWT and completes the login |
| - | `POST /api/Auth/otp/resend` | `{ email }` issues a replacement PIN for a login already in progress |
| - | `POST /api/Auth/password/change` | Authenticated; replaces the caller's password after checking the current one |

Request shapes:

```jsonc
// POST /api/Auth/otp/verify
{ "email": "jane.doe@example.test", "otp": "123456" }

// POST /api/Auth/otp/resend
{ "email": "jane.doe@example.test" }

// POST /api/Auth/password/change   (Bearer token required)
{ "currentPassword": "Old-password-123!", "newPassword": "New-password-123!" }
```

`AuthResponseDto` gained three fields and nothing was removed, so the existing token/user fields keep
their meaning once the login is complete:

| Field | Meaning |
|---|---|
| `otpRequired` | `true` while the PIN still has to be submitted |
| `otpSentTo` | Masked destination of the PIN, for example `j***e@example.test` |
| `otpExpiresInMinutes` | Validity window reported by the OTP service (null when the login is complete) |

Rules enforced by the handlers (`Features/Auth/AuthLoginPolicy.cs`):

- every role has its own password endpoint and accepts only its own accounts: students sign in with
  their student number, staff with the e-mail on their account through the Facilitator, Moderator and
  Training Provider endpoints, and each endpoint rejects accounts of any other role;
- the three staff endpoints share one implementation
  (`Features/Auth/Handlers/StaffLoginCommandHandlers.cs`), so they differ only in the role they accept;
- a deactivated SETA or System Administrator profile is rejected before a PIN is issued;
- the PIN must be exactly six digits; a wrong, expired, missing or malformed value returns 401 and
  never a token;
- `otp/verify` and `otp/resend` are anonymous by necessity because no token exists yet; a PIN is only
  ever issued after the password step of the correct role succeeded, and the PIN stays bound to the
  account e-mail address;
- an existing Base64 password is still upgraded on the password step, before the PIN is issued;
- no JWT is produced by the password step at all, so a stolen password alone is no longer enough.

## 2. Partner handover: the OTP and the E-mail

Everything the login flow needs beyond the endpoints sits behind
`DigiFikileLms.Application/Interfaces/IOtpService.cs`:

```csharp
Task<int> SendLoginOtpAsync(UserAccount user, CancellationToken cancellationToken = default);
Task<bool> VerifyLoginOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
```

`SendLoginOtpAsync` returns the number of minutes the PIN stays valid so the API can publish it.

The registered implementation is `DigiFikileLms.Infrastructure/Services/OtpService.cs` (registered in
`InfrastructureServiceExtensions.AddInfrastructureServices`). It currently throws
`NotImplementedException` with the TODO list, so the login endpoints return 500 until the PIN
generation, storage and EmailJS delivery are implemented there. The same checklist is inside that
file:

1. generate a random six-digit PIN (`RandomNumberGenerator.GetInt32(0, 1_000_000)` formatted `D6`);
2. store it for a short window keyed by the account e-mail address (EF Core table, `IMemoryCache` or
   a distributed cache — nothing else depends on the choice);
3. deliver it with `POST https://api.emailjs.com/api/v1.0/email/send` using the EmailJS service id,
   template id and public key from configuration;
4. verify in constant time, reject expired or absent PINs, then consume the PIN so it cannot be
   replayed;
5. never return the PIN through an API response and never log it.

The delivery step must stay on the server: a PIN echoed back in an HTTP response could be read by
anybody who knows the e-mail address.

The credentials mail for a provisioned administrator sits behind
`DigiFikileLms.Application/Interfaces/ICredentialsEmailService.cs`:

```csharp
Task SendTemporaryCredentialsAsync(UserAccount user, string temporaryPassword, CancellationToken cancellationToken = default);
```

Its implementation, `DigiFikileLms.Infrastructure/Services/CredentialsEmailService.cs` (registered
next to the OTP service), is the second and last handover point:

1. compose a message to `user.Email` containing the username (the same e-mail address), the
   temporary password and a pointer to `POST /api/Auth/password/change`;
2. deliver it with the EmailJS REST endpoint using the service id, template id and public key from
   configuration;
3. throw when the provider rejects the message — the provisioning handler catches that, keeps the
   account usable and returns the temporary password so the System Administrator can hand it over
   manually;
4. never log the temporary password and never send it to another address.

## 3. System Administrator registers a SETA Administrator

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/system-admin/seta-admin/provision` | `SystemAdministrator` (existing controller policy) |

Request:

```jsonc
{
  "name": "Seta",
  "surname": "Admin",
  "email": "new.seta.admin@example.test",
  "phone": "0712345678",
  "address": "Pretoria",
  "permissions": [ { "permissionName": "manage_courses", "isGranted": true } ]
}
```

Response (`BaseResponse<SetaAdminCredentialsDto>.data`):

```jsonc
// Successful delivery: the password is not in the response, it went to the account holder.
{
  "userId": 12,
  "setaAdministratorId": 7,
  "name": "Seta",
  "surname": "Admin",
  "username": "new.seta.admin@example.test",
  "email": "new.seta.admin@example.test",
  "temporaryPassword": null,
  "credentialsEmailed": true,
  "emailStatus": "The sign-in credentials were e-mailed to new.seta.admin@example.test.",
  "passwordIsTemporary": true,
  "changePasswordUrl": "/api/Auth/password/change",
  "createdAt": "2026-09-16T09:00:00Z"
}

// Provider outage: the account stays usable and the response carries the password for hand-over.
{
  "temporaryPassword": "k7#RmQ2w!xAz",
  "credentialsEmailed": false,
  "emailStatus": "The credentials e-mail could not be sent. Hand the temporary password over manually."
}
```

Notes:

- the caller does not supply a password; `ITemporaryPasswordGenerator`
  (`Infrastructure/Services/TemporaryPasswordGenerator.cs`) creates a 12-character value containing
  upper case, lower case, a digit and a symbol;
- the handler mails the credentials itself through `ICredentialsEmailService`; only the PBKDF2 hash
  is stored and the audit log records the provisioning and the mail outcome without the secret;
- because the mail happens inside the handler, `temporaryPassword` appears in the response only when
  the delivery failed — that is the manual hand-over fallback;
- `username` is the e-mail address, because SETA administrators sign in with
  `POST /api/Auth/admin/login`;
- the account holder replaces the temporary password after the first login through
  `POST /api/Auth/password/change`, and the audit uses the authenticated System Administrator
  profile id from the bearer token instead of a runtime hash;
- the existing `POST /api/system-admin/seta-admin` endpoint (caller-supplied password) is unchanged.
  Its `Permissions` payload is still not persisted anywhere; the new endpoint treats permissions the
  same way, so both remain consistent.

## 4. Files added or changed

Added (Application):

- `Interfaces/IOtpService.cs`, `Interfaces/ITemporaryPasswordGenerator.cs`,
  `Interfaces/ICredentialsEmailService.cs` (partner handover)
- `Common/EmailMasker.cs`
- `DTOs/Auth/OtpDtos.cs`, `DTOs/Auth/StaffLoginRequestDto.cs`
- `DTOs/SystemAdmin/ProvisionSetaAdminDto.cs`
- `Features/Auth/AuthLoginPolicy.cs`
- `Features/Auth/Commands/OtpCommands.cs`, `Features/Auth/Commands/StaffLoginCommands.cs`
- `Features/Auth/Handlers/OtpCommandHandlers.cs`,
  `Features/Auth/Handlers/StaffLoginCommandHandlers.cs`
- `Features/Auth/Handlers/ChangePasswordCommandHandler.cs`
- `Features/SystemAdmin/Commands/ProvisionSetaAdminCommands.cs`
- `Features/SystemAdmin/Handlers/ProvisionSetaAdminCommandHandler.cs`

Added (Infrastructure / API / docs):

- `Services/OtpService.cs` (partner handover), `Services/CredentialsEmailService.cs` (partner
  handover), `Services/TemporaryPasswordGenerator.cs`
- `docs/otp-and-seta-admin-provisioning.md`

Changed:

- `DTOs/Auth/AuthResponseDto.cs` (three additive OTP fields)
- `Features/Auth/Commands/AuthCommands.cs` (Student and SETA login now issue an OTP challenge and the
  `SystemAdministrator` login handler was added alongside them)
- `Infrastructure/Extensions/InfrastructureServiceExtensions.cs` (registers both new services)
- `API/Controllers/AuthController.cs` (four new actions)
- `API/Controllers/SystemAdminController.cs` (one new action)
- `tests/AuthChecks/Program.cs` (two-step login fixture and the new endpoint checks)

No database schema or migration was touched: the temporary-password flag is reported by the response
rather than stored, and the OTP storage decision belongs to the OTP owner.

## 5. Verification

```powershell
dotnet build DigiFikileLms.API\DigiFikileLms.API.csproj
dotnet run --project tests\AuthChecks\AuthChecks.csproj
```

`tests/AuthChecks/Program.cs` follows the two-step flow. It registers a fake `IOtpService` (a fixed
`123456` PIN) and a fake `ICredentialsEmailService` (captures what would be mailed; a `mailfail@...`
address simulates a provider outage) and verifies: the challenge response carries no token, wrong and
non-six-digit PINs are rejected, the resend endpoint answers known accounts only, the JWT is issued
by `otp/verify`, all six roles log in through their own endpoint and are rejected elsewhere, the
provisioning endpoint mails the credentials to the account holder only, an outage keeps the account
usable and returns the password for manual hand-over, the provisioned account signs in with the
e-mailed password, replaces it through `POST /api/Auth/password/change`, and the temporary password
stops working afterwards. Swagger is asserted to expose all new routes. All 101 checks pass; the only
build warnings are the pre-existing AutoMapper NU1903 advisories.