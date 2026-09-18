Backend authentication and authorization now use the existing IPasswordHasher, ITokenService and JWT Bearer infrastructure. Auth URLs and response DTOs are preserved. No frontend, database schema, migrations, or LMS action bodies were changed, apart from the auth response handling and authenticated creator identity passed into SETA account creation.

Existing roles: Student=1, Facilitator=2, Moderator=3, SetaAdministrator=4, TrainingProvider=5, SystemAdministrator=6, Assessor=7. UserRole is stored as a string. Student is the backend Learner role. Role table labels are not elevated into token roles; UserAccount.UserRole remains authoritative.

Registration validates required fields, rejects duplicate email (case-insensitive lookup) and student number, and always assigns Student. Passwords use PBKDF2-SHA512 with a random 16-byte salt, 210,000 iterations, a 32-byte result, and constant-time comparison. Existing Base64 passwords remain usable and are upgraded on successful login. Account and student profile are saved together. Unique-constraint races return 400 instead of an unhandled database error.

Student login accepts only Student accounts. POST /api/Auth/admin/login accepts only SetaAdministrator. All other roles, including SystemAdministrator and Student, receive 401 even with valid passwords. Login rejects invalid credentials with 401 and rejects inactive SETA profiles when present. No Student account activation field exists in the persisted domain model.

JWTs contain sub, NameIdentifier, email, full name, actual role, jti, not-before, expiry, issuer and audience. Email/name/role use the existing .NET claim URI types; JWT Bearer explicitly maps and recognizes ClaimTypes.Name and ClaimTypes.Role. A student_id claim is included when the student navigation is available. Tokens use HS256; validation checks signature, allowed algorithm, issuer, audience and expiry with one minute of clock skew. Default lifetime is 60 minutes. Refresh-token generation now uses random bytes; no refresh endpoint or refresh-token persistence was introduced.

Program.cs registers JWT authentication and authorization and runs UseAuthentication before UseAuthorization. The fallback policy requires authentication for actions without explicit metadata. Named policies exist for the exact backend enum roles, with no implicit administrator hierarchy. The existing Swagger extension is now called, enabling its Bearer Authorize button.

Endpoint access:
- /api/system-admin/* and /api/SystemLogs/* retain SystemAdministrator-only access.
- /api/Users/*, /api/Roles/*, /api/Permissions/* and /api/SetaAdministrators/* are SystemAdministrator-only.
- Group management, student management, moderator management, enrollment administration, certificate issuing/revocation and course publishing/archiving require SetaAdministrator. The former `/api/Admin/dashboard` endpoint was removed along with the generic Admin surface (there are two distinct administrator roles: SystemAdministrator and SetaAdministrator); administrator dashboard data now comes from `/api/system-admin/*` only.
- `/api/Complaints/*` is split by role: laying a complaint and reading your own (`/api/Complaints/mine`) require authentication only, while listing every complaint, reading one by id and updating its status are SystemAdministrator-only.
- Student profile/enrollment/progress/certificate actions, assessment submission, self-enrollment and completion actions require Student.
- Course/content/assessment authoring explicitly shares SetaAdministrator and Facilitator where supported by existing permissions; grading/pending assessments require Moderator.
- Training reports share SetaAdministrator and TrainingProvider; course enrollment/progress reads share SetaAdministrator and Facilitator.
- General authenticated reads and notification self-service require authentication.
- Auth actions, published-course listing and certificate verification remain anonymous.

SETA creation preserves the stored SetaAdministrator role and shared password hasher. The System Admin command now receives the authenticated caller ID from claims and resolves the active SystemAdministrator profile. Its audit log uses that profile's database ID instead of the previous runtime hash, avoiding an invalid audit foreign key.

Verification:
Run: dotnet run --project tests/AuthChecks/AuthChecks.csproj -c Release
The authentication regression suite covers the supported roles. They exercise real AuthController endpoints and handlers, JWT services, JWT Bearer middleware, authorization policies, real admin route rejection, Swagger, inactive profiles, legacy upgrades, and the System Admin SETA creation handler. System Admin tokens are generated directly as test fixtures; System Admin login attempts are rejected. The three-role HTTP policy matrix verifies 401 without credentials, 403 for every wrong role, and 200 for the correct role. Repositories are in-memory test doubles; PostgreSQL transactions, constraint races and the complete live database creation workflow were not integration-tested.
The Release build succeeds. Debug output is locked by the already-running API process. Existing AutoMapper NU1903 warnings remain outside this auth change.

Configuration:
- Added Jwt:Issuer, Jwt:Audience and Jwt:ExpiryMinutes to appsettings.json.
- Generated a random Jwt__SecretKey in the ignored local .env, without printing or committing it.
- Other environments must provide their own stable Jwt__SecretKey (at least 32 UTF-8 bytes).
- Restart/rebuild the running API to load these changes.
- The pre-existing deletion of .env.example was left untouched.

Remaining limits:
- This change enforces endpoint roles. Existing per-record ownership and organization/tenant checks were not redesigned; a role check alone does not establish ownership of a supplied ID.
- Disabling a SETA admin blocks subsequent login. Already-issued JWTs remain valid until expiry; no revocation store was added.
- System Admin has no login endpoint in the current API; admin/login is reserved for SETA administrators. Student activation status is not modeled by existing backend flows.
- Legacy Base64 credentials are upgraded only when those users log in.
- Email checks normalize comparison, but the unchanged database unique index is case-sensitive; simultaneous registrations with case variants still require database-level normalization for a strict concurrency guarantee.

Modified files:
- DigiFikileLms.API/Controllers/AdminController.cs
- DigiFikileLms.API/Controllers/AssessmentsController.cs
- DigiFikileLms.API/Controllers/AuthController.cs
- DigiFikileLms.API/Controllers/CertificatesController.cs
- DigiFikileLms.API/Controllers/CoursesController.cs
- DigiFikileLms.API/Controllers/EnrollmentsController.cs
- DigiFikileLms.API/Controllers/GroupsController.cs
- DigiFikileLms.API/Controllers/LessonsController.cs
- DigiFikileLms.API/Controllers/ModeratorsController.cs
- DigiFikileLms.API/Controllers/ModulesController.cs
- DigiFikileLms.API/Controllers/NotificationsController.cs
- DigiFikileLms.API/Controllers/PermissionsController.cs
- DigiFikileLms.API/Controllers/ProgressController.cs
- DigiFikileLms.API/Controllers/ReportsController.cs
- DigiFikileLms.API/Controllers/RolesController.cs
- DigiFikileLms.API/Controllers/SetaAdministratorsController.cs
- DigiFikileLms.API/Controllers/StudentsController.cs
- DigiFikileLms.API/Controllers/SystemAdminController.cs
- DigiFikileLms.API/Controllers/UsersController.cs
- DigiFikileLms.API/Program.cs
- DigiFikileLms.API/appsettings.json
- DigiFikileLms.Application/Features/Auth/Commands/AuthCommands.cs
- DigiFikileLms.Application/Features/SystemAdmin/Commands/SystemAdminCommands.cs
- DigiFikileLms.Application/Features/SystemAdmin/Handlers/SystemAdminHandlers.cs
- DigiFikileLms.Application/Interfaces/IPasswordHasher.cs
- DigiFikileLms.Domain/Enums/UserRole.cs
- DigiFikileLms.Infrastructure/Extensions/AuthenticationExtensions.cs
- DigiFikileLms.Infrastructure/Persistence/Repositories/UserAccountRepository.cs
- DigiFikileLms.Infrastructure/Services/LmsServices.cs
- .env (ignored local signing key)
- tests/AuthChecks/AuthChecks.csproj (new)
- tests/AuthChecks/Program.cs (new)
- docs/authentication-authorization.md (new)
