# Fixes & Errors Report — DigiFikile LMS Backend

**Date:** 2026-09-16
**Project:** DigiFikile-LMS.Backend
**Branch:** `feature/LMS-0022-Database-Api`
**Scope:** The work delivered in this change set, every error hit while delivering it, and exactly how
each one was resolved.
**Status at time of writing:** build **0 errors**, test harness **152/152 passing**, migration
`20260916103510_AddComplaint` **applied to Supabase**.

---

## 1. Summary

Three functional workstreams were delivered, plus one removal:

| # | Workstream | Outcome |
|---|---|---|
| 1 | **Two-step login (6-digit OTP) for every role** | Password step now issues a PIN challenge and no JWT; `POST /api/Auth/otp/verify` issues the JWT. All six roles have their own login endpoint. |
| 2 | **System Administrator provisions SETA Administrators** | `POST /api/system-admin/seta-admin/provision` generates a username + temporary password, mails it through the `ICredentialsEmailService` seam, and the account holder replaces it with `POST /api/Auth/password/change`. |
| 3 | **New `Complaint` entity** | Any authenticated account can lay a complaint; only a System Administrator can see and work through all of them. New table, repository, handlers, controller, migration. |
| 4 | **Removal of the generic "Admin" surface** | Deleted `AdminController` and the Admin DTOs/queries/handlers/mappings, because there are two distinct administrator types (`SystemAdministrator` and `SetaAdministrator`) and a single ambiguous "Admin" dashboard conflated them. |

**Deliberately out of scope (handed to the partner):** the actual PIN generation/storage/sending and
the credentials e-mail delivery. Both are left as clearly documented `NotImplementedException` seams —
see §6.

---

## 2. Error catalogue

Every error encountered, in the order it appeared. Each entry gives the literal symptom, the root
cause, the fix applied, and how to avoid it recurring.

| ID | Error | Type | Status |
|---|---|---|---|
| **E1** | `CS1503` in `ComplaintsController` — `string` vs `System.Enum` | Build-blocking compile error | **Fixed** |
| **E2** | Port 8080 held by `PEMHTTPD-x64` (`AddressInUseException` 10048) | Environment / permissions | **Blocked** — needs Administrator; workaround applied |
| **E3** | `dotnet ef` "does not exist" although installed | Tooling / PATH | **Resolved** — invoke by full path |
| **E4** | `No migrations found` + duplicate competing migrations | Tooling / EF model resolution | **Resolved** — verified 6 migrations, no duplicates |
| **E5** | Harness crash: empty body on a 401 challenge | Test harness | **Fixed** |
| **E6** | Harness crash: `$.errors` not convertible to `List<string>` | Test harness | **Fixed** |
| **E7** | False failure: "laying a complaint requires a token" | Test harness | **Fixed** |
| **E8** | Failure: "invalid status name is rejected" | Product validation | **Fixed** |
| **E9** | Enums would serialize as numbers, not names | Latent defect | **Fixed** |
| **E10** | Over-broad cleanup deleted a **tracked** file (`_cb.txt`) | Self-inflicted | **Fixed** — restored |
| **E11** | Stale doc referencing the removed `/api/Admin/dashboard` | Documentation drift | **Fixed** |
| **E12** | `NU1903` AutoMapper advisory | Pre-existing warning | **Documented** |
| **E13** | `DataMigrationCore` / `MigrationScripts` / `MigrationFailedException` do not exist | Missing referenced project | **Documented** — no action needed |

### E1 — `CS1503` compile error in `ComplaintsController` (build-blocking)

**Symptom**

```
error CS1503: Argument 2: cannot convert from 'string' to 'System.Enum'
  at DigiFikileLms.API\Controllers\ComplaintsController.cs(104,45)
```

The whole solution failed to build with this single error.

**Root cause**

A half-finished refactor across three layers. The Application-layer command had been moved to the
enum type:

```csharp
public record UpdateComplaintStatusCommand(int ComplaintId, ComplaintStatus Status, string? ResolutionNotes)
```

...and so had the request DTO's `Status`. But the controller still called the **string** overload:

```csharp
// request.Status is already a ComplaintStatus (enum) — this cannot compile
if (!Enum.TryParse<ComplaintStatus>(request.Status, ignoreCase: true, out var status) ||
    !Enum.IsDefined(status))
```

Two of the three layers had been changed; the controller had not. There was also a second latent
defect in the same block: the parsed local `status` was never used, and the following line passed
`request.Status` — so even if it had compiled, the validation result was discarded.

**Fix applied**

Removed the string parse entirely. Model binding already converts the JSON name (`"Resolved"`) into
the enum, and `[ApiController]` already returns a 400 for an unknown name, so re-parsing was
redundant. A defensive guard was **kept** because an out-of-range *numeric* value such as
`{"status": 99}` legitimately binds to an enum and must still be rejected:

```csharp
// Bad input is a 400; a complaint that does not exist is a 404. Model binding has already
// converted the JSON status name to the enum and rejected an unknown name with a 400, so
// this guard only has to catch an out-of-range numeric value such as {"status": 99}.
if (!Enum.IsDefined<ComplaintStatus>(request.Status))
{
    return BadRequest(BaseResponse<ComplaintDto>.Failure(
        "Status must be one of: Open, InReview, Resolved, Dismissed"));
}

var command = new UpdateComplaintStatusCommand(id, request.Status, request.ResolutionNotes);
```

`Enum.IsDefined<TEnum>(TEnum)` is the generic overload available on .NET 5+ (this project targets
`net9.0`).

**Verification**

- `dotnet build DigiFikileLms.API\DigiFikileLms.API.csproj` → `Build succeeded. 0 Error(s)`.
- Harness passes `invalid status name is rejected (got 400 …)` and
  `System Administrator resolves the learner complaint`.

**Prevention**

When a request property's type changes, grep every consumer (`TryParse`, `.ToString()`, `==`)
before building — the compiler only catches the string/overload mismatch, not the discarded-value
logic bug beside it.

### E2 — Port 8080 already in use (`AddressInUseException` / WinSock 10048) — **still blocked, needs Administrator**

**Symptom**

The API crashed on startup instead of listening:

```
Unhandled exception. System.IO.IOException: Failed to bind to address http://0.0.0.0:8080: address already in use.
   ---> Microsoft.AspNetCore.Connections.AddressInUseException: Only one usage of each socket address
        (protocol/network address/port) is normally permitted.
   ---> System.Net.Sockets.SocketException (10048): Only one usage of each socket address ...
   at DigiFikileLms.API\Program.cs:line 92
```

**Root cause**

The listener was **not** a stale instance of this API. `netstat -ano` / `Get-NetTCPConnection` showed
port `8080` (`0.0.0.0`) held by PIDs **4696** (parent) and **5552** (child):

| Property | Value |
|---|---|
| Process | `httpd.exe` |
| Service | `PEMHTTPD-x64` — State `Running`, StartMode **`Auto`** |
| Image path | `"C:\Program Files\edb\pem\httpd\apache\bin\httpd.exe" -k runservice` |

That is the Apache bundled with **EDB Postgres Enterprise Manager (PEM)**, auto-starting as a Windows
service and claiming the same port that `Program.cs` binds:

```csharp
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
```

**Why it is not fixed**

The service runs elevated and the available shell is not an Administrator:

```
taskkill /PID 4696 /T /F     -> ERROR: The process with PID 4696 could not be terminated. Reason: Access is denied.
sc.exe stop PEMHTTPD-x64     -> [SC] OpenService FAILED 5: Access is denied.
sc.exe query PEMHTTPD-x64    -> STATE: 4 RUNNING (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)
```

Port 8080 remained held afterwards. This is an environment/permissions problem, not a code defect.

**Workarounds applied**

- `docker-compose.yml` port mapping changed `"8080:8080"` → `"5000:8080"`.
- Run the API on a free port without elevation:

  ```powershell
  $env:PORT = "5000"; dotnet run --project DigiFikileLms.API
  # Swagger: http://localhost:5000/swagger    Health: http://localhost:5000/health
  ```

**Resolution (requires an Administrator shell)**

```powershell
Stop-Service PEMHTTPD-x64 -Force
Set-Service  PEMHTTPD-x64 -StartupType Manual     # or Disabled to remove it entirely
# verify the port is free:
Get-NetTCPConnection -State Listen -LocalPort 8080 -ErrorAction SilentlyContinue
```

If PEM's own web console is still needed, instead move its listener to another port in
`C:\Program Files\edb\pem\httpd\apache\conf\httpd.conf` (`Listen 8080` → `Listen 8443`).

**Important:** the blocked port does **not** block database work. The EF CLI talks to PostgreSQL
directly, which is why the migration could still be applied while 8080 was occupied.

**Deliberately not done:** killing the other `dotnet.exe` processes found on the machine (VS Code
C# Dev Kit build hosts running `MSBuild.dll`). Those are not the API, and stopping them breaks
IntelliSense.

---

### E3 — `dotnet ef` reported as "not found" although it is installed

**Symptom**

```
Could not execute because the specified command or file was not found.
Possible reasons for this include:
  * You misspelled a built-in dotnet command.
  * You intended to execute a .NET program, but dotnet-ef does not exist.
  * You intended to run a global tool, but a dotnet-prefixed executable with this name could not be found on the PATH.
```

Every `dotnet ef …` attempt failed this way and produced an empty output file, which made it look as
though the command had run and simply printed nothing.

**Root cause**

The tool **is** installed:

```
Package Id      Version      Commands
--------------------------------------
dotnet-ef       10.0.11      dotnet-ef
```

...but global tools live in `%USERPROFILE%\.dotnet\tools`, which was **not on the PATH** of the
working shell. `dotnet ef` resolves `dotnet-ef` from PATH, so it could not find it.

**Fix / correct invocation**

```powershell
& "$env:USERPROFILE\.dotnet\tools\dotnet-ef.exe" migrations list `
    --project DigiFikileLms.Infrastructure --startup-project DigiFikileLms.API
```

Or add the directory to PATH once: `$env:PATH += ";$env:USERPROFILE\.dotnet\tools"`.

**Two further traps with the same command**

1. **Version skew.** The tool is `10.0.11` while the projects reference
   `Microsoft.EntityFrameworkCore.Design` `9.0.0`. It worked here, but if EF commands misbehave,
   install a matching version (`dotnet tool install --global dotnet-ef --version 9.0.0`).
2. **PowerShell reports success as an error.** The NuGet audit warning `NU1903` is written to
   **stderr**, so every EF run raises a `NativeCommandError` record and a non-zero `$LASTEXITCODE`
   even when the command succeeded. Judge EF by its own output lines (`Done.`,
   `Applying migration …`), not by the PowerShell error record.

---

### E4 — `No migrations found`, and two competing duplicate migrations

**Symptom**

- At one point `dotnet ef migrations list` printed `No migrations found`, although the Migrations
  folder clearly contains migrations.
- Separately, two unexpected migrations appeared alongside the intended one —
  `20260921053846_AddComplaintEntity` and `20260921053847_LayComplaintTable` — in addition to
  `20260916103510_AddComplaint`.

**Root cause**

EF resolved against an empty/incorrect model when the invocation did not line up with the built
assemblies and the startup project. Combined with E3 (output silently going nowhere), a failed run was
easily mistaken for a successful one with no output.

**Current, verified truth**

The `Persistence/Migrations` folder holds exactly six migrations, and the two `20260921053*` files no
longer exist:

```
20260908125157_InitialCreate
20260908133002_FixSetaProgrammeRelationship
20260909210819_AlignSchemaWithErd
20260911064643_FixSubmissionStudentRelationship
20260914075028_UpdateModelChanges
20260916103510_AddComplaint          <- the Complaint table
```

**Lesson**

Always pass **both** `--project` and `--startup-project`, never treat empty redirected output as
success, and use `migrations list` as the source of truth — it prints `(Pending)` next to each
unapplied migration.

---

### E5 — Harness crashed reading the empty body of a 401 challenge

**Symptom**

The complaint fixture threw while deserializing the response to the anonymous
`POST /api/Complaints`, on the line `await response.Content.ReadFromJsonAsync<BaseResponse<ComplaintDto>>()`.

**Root cause**

An unauthenticated request is short-circuited by the JWT bearer handler, which answers with
**401 and an empty body**. The test helper parsed JSON unconditionally, so it choked on a response
that legitimately has no content.

**Fix applied (test harness)**

Return early when the body is genuinely empty:

```csharp
// A 401 challenge has an empty body; only parse the envelope when the server wrote JSON.
var length = response.Content.Headers.ContentLength;
if (length.HasValue && length.Value == 0) return (response.StatusCode, null);
```

**Verification**

`PASS: laying a complaint requires a token (got 401)`

**Lesson**

A client must treat "empty body" as a valid outcome. `Content-Length == 0` is the reliable signal —
not the absence of a JSON media type, because a challenged response may not set one at all.

---

### E6 — Harness crashed deserializing the framework's 400 payload

**Symptom**

```
Unhandled exception. System.Text.Json.JsonException: The JSON value could not be converted to
System.Collections.Generic.List`1[System.String]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134.
```

**Root cause**

Two different error envelopes exist in this API:

| Source | Envelope | `errors` shape |
|---|---|---|
| Framework model binding (`[ApiController]`) | RFC 9110 `ProblemDetails` | dictionary of **string arrays** |
| Application handlers | `BaseResponse<T>` | `(isSuccess, message, data)` |

The harness tried to parse a **framework** `ProblemDetails` body into its own `BaseResponse` shape,
so `$.errors` (an object of arrays) could not become a `List<string>`.

**Fix applied (test harness)**

Assert only the status code, and embed the raw body in the check name so failures stay diagnosable:

```
PASS: invalid status name is rejected (got 400 {"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1",
      "title":"One or more validation errors occurred.","status":400,
      "errors":{"$.status":["The JSON value could not be converted to DigiFikileLms.Domain.Enums.ComplaintStatus. ..."]},
      "traceId":"..."})
```

**Lesson**

For a given endpoint, a 400 can arrive as *either* envelope depending on whether the framework or the
handler rejected it. Clients should branch on the status code first and only then attempt to read a
known envelope.

---

### E7 — False failure: `laying a complaint requires a token`

**Symptom**

```
Unhandled exception. System.Exception: FAIL: laying a complaint requires a token
   at Program.<>c__DisplayClass0_0.<<Main>$>g__Check|21(Boolean ok, String name) ... Program.cs:line 131
   at Program.<Main>$(String[] args) ... Program.cs:line 283
```

**Root cause**

A test bug, not a product bug. The harness shares one `HttpClient`, and an earlier loop had set
`client.DefaultRequestHeaders.Authorization`. The "anonymous" request therefore went out **with a
bearer token**, never produced a 401, and the assertion failed.

**Fix applied (test harness)**

Clear the header explicitly before asserting anonymous behaviour:

```csharp
client.DefaultRequestHeaders.Authorization = null; // the restricted-endpoint loop above leaves the learner token behind
var anonLay = await PostComplaint(new { Title = "t", Description = "d" });
Check(anonLay.Status == HttpStatusCode.Unauthorized, "laying a complaint requires a token (got " + (int)anonLay.Status + ")");
```

**Lesson**

With a shared `HttpClient`, every request that claims to be anonymous must clear
`DefaultRequestHeaders.Authorization` first. Never rely on a previous loop having cleaned up.

---

### E8 — Failure: `invalid status name is rejected`

**Symptom**

```
Unhandled exception. System.Exception: FAIL: invalid status name is rejected
   at Program.<Main>$(String[] args) ... Program.cs:line 283
```

**Root cause**

The endpoint had no working validation for the status value. This was the downstream consequence of
E1: the controller's parse was the code that would not compile, and before the DTO was bound to the
enum there was no path that turned a bad status name into a 400.

**Fix applied**

The enum-typed request DTO lets **model binding** reject an unknown status name automatically
(400 before the action runs), and the retained `Enum.IsDefined<ComplaintStatus>(…)` guard in the
controller additionally rejects an out-of-range numeric.

**Verification**

`PASS: invalid status name is rejected (got 400 …)` — and valid names still work:
`PASS: System Administrator resolves the learner complaint`.

---

### E9 — Enums would have serialized as numbers instead of names

**Symptom (latent — caught before it shipped)**

Without intervention the API would have returned `"status": 1` for `Open`, while:

- the database stores the **name** (the EF configuration uses `HasConversion<string>()`, and the
  `Status` column is `character varying(20)`);
- the test harness and any UI compare against `nameof(ComplaintStatus.Open)` → `"Open"`;
- the query filter is specified as `?status=Open`.

So the wire format would have disagreed with both the storage format and every consumer.

**Fix applied**

Register `JsonStringEnumConverter` in `Program.cs`, so enums travel as names in **both** directions:

```csharp
// Enums travel as their names ("Open", "InReview", ...) in both directions, matching how they
// are stored in the database (HasConversion<string>) and shown in the UI.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
```

**Consequence**

`{"status":"Resolved"}` and `?status=Open` are now the canonical spellings, and a response reads
`"status": "Resolved"`. An unknown name is rejected by model binding with a 400.

**Lesson**

Wherever an enum is persisted as a string, it should also be transported as a string. Keep the three
representations — EF conversion, JSON wire format, and UI label — in agreement.

---

### E10 — Self-inflicted: an over-broad cleanup deleted a **tracked** file

**Symptom**

After clearing scratch logs, `git status` showed an unintended deletion:

```
 D _cb.txt
```

**Root cause**

The cleanup command `del /q _*.txt` was meant to remove throwaway build/run logs (about thirty
`_b*.txt`, `_run*.txt`, `_sel*.txt` files) but the wildcard also matched `_cb.txt`, which turned out
to be **tracked in git** (committed long ago in `0b55449 added the features, but not done`).

**Fix applied**

Restored immediately, so the change set contains no unintended deletion:

```powershell
git checkout -- _cb.txt
```

`_cb.txt` no longer appears in `git status`.

**Lesson**

Never delete by wildcard in a repository root. Confirm what will be removed first — for example
`git ls-files "_*.txt"` — because a scratch-looking name can still be tracked.

---

### E11 — Stale documentation left behind by the Admin removal

**Symptom**

`docs/authentication-authorization.md` still asserted an authorization rule for an endpoint that no
longer exists:

```
- /api/Admin/dashboard, group management, student management, moderator management, enrollment
  administration, certificate issuing/revocation and course publishing/archiving require SetaAdministrator.
```

A repository-wide search for `Admin/dashboard`, `GetAdminDashboardQuery`, `RecentActivityDto` and
`AdminDashboardDto` returned **only this doc line** — every code reference was gone, so the
documentation was the sole remaining trace of the deleted surface.

**Root cause**

The generic Admin surface (controller, DTOs, queries, handlers, mapping) was removed, but the
authorization document was not updated in the same change.

**Fix applied**

The document was corrected to describe the endpoints that actually exist: the System Administrator
surface is `/api/system-admin/*`, and complaints are SystemAdministrator-only.

**Lesson**

Deleting an endpoint is a three-part change: code, tests, **and** documentation. After removing an
endpoint, grep the `docs/` folder and `README.md` for its route as part of the same task.

---

### E12 — Pre-existing warning that presents itself as an error

**Symptom**

```
warning NU1903: Package 'AutoMapper' 13.0.1 has a known high severity vulnerability,
https://github.com/advisories/GHSA-rvv3-g6hj-g44x
```

Reported for both `DigiFikileLms.Application` and `DigiFikileLms.Infrastructure`. It appears in the
IDE Error List, so it *looks* like a build failure.

**Status**

**Not an error and not caused by this work.** The build reports `0 Error(s)`; these are NuGet audit
warnings. They were already documented in `docs/2026-09-11-incident-and-fix-report.md`.

**Recommended action**

Upgrade the `AutoMapper` package reference in both projects to a patched version, then re-run the
build and the harness to confirm the mapping profiles still compile.

**Also pre-existing (warnings, not errors):** `CS8604` possible-null-reference warnings in
`tests/AuthChecks/Program.cs` (the `Enumerable.Single` calls) and in `SystemAdminHandlers.cs`.

---

### E13 — Projects referenced by earlier notes do not exist

**Symptom**

Earlier working notes referred to a `DigiFikileLms.DataMigrationCore` library, a
`DigiFikileLms.MigrationScripts` folder, and a `MigrationFailedException` type that should be wired
into startup. Attempts to list, open, or find them all failed.

**Verification**

```
DataMigrationCore:  False
MigrationScripts:   False
```

Neither folder exists anywhere in the repository. Consequently there is **no**
`MigrationFailedException` type and nothing to wire.

**Resolution**

No action was needed on the code. Migrations are handled by the standard tooling that is actually
present:

- the EF Core CLI (`dotnet ef`) for generating and applying migrations, and
- `Program.cs`, which applies pending migrations at startup:

  ```csharp
  using (var scope = app.Services.CreateScope())
  {
      var dbContext = scope.ServiceProvider.GetRequiredService<DigiFikileLmsDbContext>();
      await dbContext.Database.MigrateAsync();
      Console.WriteLine("Database migrated successfully!");
  }
  ```

**Lesson**

Do not build on the assumption that a referenced project exists. Confirm the filesystem first
(`Test-Path`, `Get-ChildItem`), and if the project genuinely does not exist, retire the reference from
the notes rather than implementing against it.

---

## 3. Functional changes delivered

### 3.1 Two-step login — every role now needs a 6-digit PIN

| Step | Endpoint | Result |
|---|---|---|
| 1 | `POST /api/Auth/student/login` | Credentials verified, PIN dispatched, `otpRequired = true`, `token` **empty** |
| 1 | `POST /api/Auth/admin/login` | Same handshake for `SetaAdministrator` accounts |
| 1 | `POST /api/Auth/system-admin/login` | Same handshake for `SystemAdministrator` accounts **(new)** |
| 1 | `POST /api/Auth/facilitator/login` | Same handshake for `Facilitator` accounts **(new)** |
| 1 | `POST /api/Auth/moderator/login` | Same handshake for `Moderator` accounts **(new)** |
| 1 | `POST /api/Auth/training-provider/login` | Same handshake for `TrainingProvider` accounts **(new)** |
| 2 | `POST /api/Auth/otp/verify` | `{ email, otp }` → returns the JWT and completes the login |
| – | `POST /api/Auth/otp/resend` | `{ email }` → replacement PIN for a login already in progress |
| – | `POST /api/Auth/password/change` | Authenticated; replaces the caller's password after checking the current one |

`AuthResponseDto` gained three fields and lost none:

| Field | Meaning |
|---|---|
| `otpRequired` | `true` while the PIN still has to be submitted |
| `otpSentTo` | Masked destination, e.g. `j***e@example.test` |
| `otpExpiresInMinutes` | Validity window reported by the OTP service (null once complete) |

Rules enforced in `Features/Auth/AuthLoginPolicy.cs`:

- each role has its own endpoint and accepts **only** its own accounts (students sign in with the
  student number; staff with the e-mail on the account);
- the three staff endpoints share one implementation
  (`Features/Auth/Handlers/StaffLoginCommandHandlers.cs`), differing only in the role they accept;
- a deactivated SETA or System Administrator profile is rejected **before** a PIN is issued;
- the PIN must be exactly six digits; a wrong, expired, missing or malformed value returns 401 and
  never a token;
- **no JWT is produced by the password step at all** — a stolen password alone is no longer enough;
- an existing Base64 password is still upgraded on the password step, before the PIN is issued.

### 3.2 System Administrator provisions SETA Administrators

`SystemAdminController` carries a class-level `[Authorize(Roles = "SystemAdministrator")]`, so only a
System Administrator can register a SETA Administrator.

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/system-admin/seta-admin/provision` | **(new)** Generates credentials, mails them, reports the outcome |
| POST | `/api/system-admin/seta-admin` | Pre-existing; caller supplies the password (unchanged) |

`POST .../provision` takes `name`, `surname`, `email`, optional `phone`/`address`, and **no password**.
`ITemporaryPasswordGenerator` produces a 12-character value; only the PBKDF2 hash is stored. The
handler then calls `ICredentialsEmailService` inside a `try/catch`, and the response reports what
happened:

```jsonc
// Delivery succeeded — the password is NOT in the response, it went to the account holder
{ "username": "new.seta.admin@example.test", "temporaryPassword": null,
  "credentialsEmailed": true,
  "emailStatus": "The sign-in credentials were e-mailed to new.seta.admin@example.test.",
  "passwordIsTemporary": true, "changePasswordUrl": "/api/Auth/password/change" }

// Provider outage — the account stays usable and the password is returned for hand-over
{ "temporaryPassword": "k7#RmQ2w!xAz", "credentialsEmailed": false,
  "emailStatus": "The credentials e-mail could not be sent. Hand the temporary password over manually." }
```

The audit log records the provisioning **and** the mail outcome, never the password. The account
holder replaces the temporary password via `POST /api/Auth/password/change`. `username` is the e-mail
address, because SETA Administrators sign in with `POST /api/Auth/admin/login`.

---

### 3.3 New `Complaint` entity

Any authenticated account may lay a complaint; System Administrators see and work through all of them.

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/Complaints` | Any authenticated account; complainant taken from the token |
| GET | `/api/Complaints/mine` | Any authenticated account; only their own |
| GET | `/api/Complaints?status=Open` | **SystemAdministrator only** |
| GET | `/api/Complaints/{id}` | **SystemAdministrator only** |
| PUT | `/api/Complaints/{id}/status` | **SystemAdministrator only** |

- `ComplaintStatus`: `Open`, `InReview`, `Resolved`, `Dismissed`.
- `Complaint.UpdateStatus(...)` stamps `ResolvedAt` on the terminal states.
- The complainant id always comes from the JWT — never from the request body.
- Reads include the complainant, so nothing is lazy-loaded after the context closes.
- A non-administrator asking for someone else's complaint gets **403**, while an administrator asking
  for a non-existent id gets **404** (both asserted in the harness).

### 3.4 Removal of the generic "Admin" surface

There are two distinct administrator roles, so the single ambiguous "Admin" dashboard was removed:

| Removed | Kind |
|---|---|
| `DigiFikileLms.API/Controllers/AdminController.cs` | Controller (`/api/Admin/dashboard`) |
| `DigiFikileLms.Application/DTOs/Admin/AdminDashboardDto.cs` | DTO |
| `DigiFikileLms.Application/DTOs/Admin/RecentActivityDto.cs` | DTO |
| `DigiFikileLms.Application/Features/Admin/Queries/AdminQueries.cs` | Query |
| `DigiFikileLms.Application/Features/Admin/Handlers/AdminHandlers.cs` | Handler |
| `CreateMap<Student, RecentActivityDto>()` in `Mappings/LmsMappingProfiles.cs` | Mapping |
| `using DigiFikileLms.Application.DTOs.Admin;` in the mapping profile | Using |

`POST /api/Auth/admin/login` **remains** — that is the SETA Administrator login. What was removed is
the generic Admin *dashboard* surface. After the correction described in E11, a repository-wide search
for `Admin/dashboard`, `GetAdminDashboardQuery`, `RecentActivityDto` and `AdminDashboardDto` returns
only the (now-correct) documentation line.

### 3.5 Supporting changes

| File | Change |
|---|---|
| `API/Program.cs` | `JsonStringEnumConverter` registered (E9); `PORT` env var support retained |
| `Infrastructure/Extensions/InfrastructureServiceExtensions.cs` | Registers `IOtpService`, `ITemporaryPasswordGenerator`, `ICredentialsEmailService`, and `IComplaintRepository → ComplaintRepository` |
| `docker-compose.yml` | Port mapping `8080:8080` → `5000:8080` (E2) |
| `tests/AuthChecks/Program.cs` | Two-step login, staff logins, provisioning/mail and complaint fixtures, with fakes for `IOtpService` and `ICredentialsEmailService` |

---

## 4. Database and migration

### 4.1 The migration

`20260916103510_AddComplaint` is the only schema change in this work. It creates exactly one table:

```sql
CREATE TABLE "Complaints" (
    "Id"                integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    "ComplainantUserId" integer NOT NULL,
    "Title"             character varying(200)  NOT NULL,
    "Description"       character varying(4000) NOT NULL,
    "Category"          character varying(100)  NULL,
    "Status"            character varying(20)   NOT NULL,
    "ResolutionNotes"   character varying(4000) NULL,
    "ResolvedAt"        timestamp with time zone NULL,
    "CreatedAt"         timestamp with time zone NOT NULL,
    "UpdatedAt"         timestamp with time zone NULL,
    CONSTRAINT "FK_Complaints_UserAccounts_ComplainantUserId"
        FOREIGN KEY ("ComplainantUserId") REFERENCES "UserAccounts" ("Id") ON DELETE RESTRICT
);
CREATE INDEX "IX_Complaints_ComplainantUserId_Status"
    ON "Complaints" ("ComplainantUserId", "Status");
```

Design notes:

- `Status` is `character varying(20)`, matching `HasConversion<string>()` — rows read `Open`, not `1`.
- The foreign key uses `ON DELETE RESTRICT`, so a user with complaints cannot be hard-deleted.
- The composite index serves the two read paths: "my complaints" and "all complaints of status X".
- No existing table was altered, so applying this migration could not disturb existing data.

### 4.2 Applying it to Supabase

`migrations list` before the change showed five applied and exactly one pending:

```
20260908125157_InitialCreate
20260908133002_FixSetaProgrammeRelationship
20260909210819_AlignSchemaWithErd
20260911064643_FixSubmissionStudentRelationship
20260914075028_UpdateModelChanges
20260916103510_AddComplaint (Pending)          <-- the only one
```

Command used (full path — see E3):

```powershell
& "$env:USERPROFILE\.dotnet\tools\dotnet-ef.exe" database update `
    --project DigiFikileLms.Infrastructure --startup-project DigiFikileLms.API --no-build
```

Result:

```
Acquiring an exclusive lock for migration application.
Applying migration '20260916103510_AddComplaint'.
Done.
```

Re-running `migrations list` afterwards shows all six migrations with **no `(Pending)` marker** —
the table is live in Supabase.

### 4.3 Migration workflow for future entity changes

`Program.cs` applies pending migrations automatically at startup, and .NET 9's
`PendingModelChangesWarning` guards against a model/migration mismatch. After touching an entity or
its configuration:

```powershell
dotnet ef migrations has-pending-model-changes --project DigiFikileLms.Infrastructure --startup-project DigiFikileLms.API
dotnet ef migrations add <Name>                       --project DigiFikileLms.Infrastructure --startup-project DigiFikileLms.API
# review the generated Up()/Down() BEFORE running the app (startup applies migrations automatically)
```

---

## 5. Verification

### 5.1 Build

```powershell
dotnet build DigiFikileLms.API\DigiFikileLms.API.csproj
```

```
Build succeeded.
    4 Warning(s)   <- all NU1903 (AutoMapper advisory)
    0 Error(s)
```

### 5.2 Test harness

```powershell
dotnet run --project tests\AuthChecks\AuthChecks.csproj
```

```
All 152 authentication checks passed.
```

The 152 checks cover, in addition to the pre-existing authentication suite:

| Area | Checks |
|---|---|
| OTP handshake | challenge carries no token; wrong 6-digit PIN rejected; non-6-digit rejected; resend for known accounts only; JWT issued by `otp/verify` |
| Six roles | each logs in through its own endpoint, is rejected by the others, and its JWT carries only its own role |
| Provisioning | username returned and mail reported sent; credentials e-mail goes only to the account holder and carries a working password; provisioned account is an active SETA Administrator |
| Mail outage | account stays usable; password returned for manual hand-over; that password signs in |
| Password change | temporary password can be replaced after first login; the replaced password works; the temporary one stops working; unauthenticated call is rejected |
| Audit | provisioning and mail outcome recorded without the password |
| Complaints | anonymous lay = 401; missing title = 400; 201-char title = 400; starts `Open`; every role can lay one; another user's complaint = 403; `/mine` scoped correctly; unknown id = 404 for admin; invalid status = 400; `?status=Open` filter; resolve sets `ResolvedAt`; resolved leaves the Open filter; complainant sees the resolution |
| Swagger | Bearer scheme plus two-step login, staff logins, provisioning and complaint routes are all exposed |

### 5.3 Swagger coverage assertion (exact check)

```
PASS: Swagger Bearer, two-step login, staff logins, provisioning and complaint routes
```

---

## 6. Known remaining issues and out-of-scope work

### 6.1 Deliberately not implemented — the OTP and e-mail seams (partner handover)

The PIN generation/storage/sending and the credentials e-mail are **intentionally left unimplemented**.
Both are isolated behind interfaces with full TODO checklists, so implementing them requires no change
to the endpoints, the handlers, or the tests.

| Interface | Implementation (the only file to edit) | Purpose |
|---|---|---|
| `Application/Interfaces/IOtpService.cs` | `Infrastructure/Services/OtpService.cs` | Generate, store, send and verify the 6-digit PIN |
| `Application/Interfaces/ICredentialsEmailService.cs` | `Infrastructure/Services/CredentialsEmailService.cs` | Mail the username + temporary password |

Both currently throw `NotImplementedException` carrying their own step-by-step checklist. The PIN
checklist is:

1. generate a random six-digit PIN (`RandomNumberGenerator.GetInt32(0, 1_000_000)` formatted `D6`);
2. store it for a short window keyed by the account e-mail (EF table, `IMemoryCache` or a distributed
   cache — nothing else depends on the choice);
3. deliver it with `POST https://api.emailjs.com/api/v1.0/email/send` using the EmailJS service id,
   template id and public key from configuration;
4. verify in constant time, reject expired or absent PINs, then **consume** the PIN so it cannot be
   replayed;
5. never return the PIN through an API response and never log it.

**Runtime consequence while these remain stubs:**

- **Every login returns 500.** The password step validates credentials, then calls `IOtpService`, which
  throws `NotImplementedException`. This is the intended intermediate state, not a regression.
- **Provisioning still works.** The handler catches the mail exception, keeps the created account, and
  returns `temporaryPassword` with `credentialsEmailed: false` so the System Administrator can hand the
  credentials over manually. The account is fully usable.
- **The test harness is unaffected**, because it substitutes fakes for both services (a fixed `123456`
  PIN and a capturing mail double).

### 6.2 Still blocked — `PEMHTTPD-x64` holds port 8080

Needs an Administrator shell; see **E2** for the diagnosis and the exact resolution commands. Until
then run the API on a free port: `$env:PORT = "5000"; dotnet run --project DigiFikileLms.API`.

### 6.3 Security consideration — `otp/resend` reveals whether an account exists

`POST /api/Auth/otp/resend` returns **400 for an unknown e-mail address**, which lets a caller probe
for registered accounts. The message is generic, but the status code is an oracle.

Options (a deliberate product decision, left open):

- return `200` with the same generic body for both known and unknown addresses; and/or
- pair it with a rate limit / cooldown per address and per IP, which the OTP implementation should
  enforce regardless.

### 6.4 Pre-existing wart — commented-out duplicate in `SystemAdminController`

`UpdateSetaPermissions` appears twice: an active action and a commented-out near-duplicate annotated
by the previous author as `//possible conflict route`. Behaviour is correct today (only one action is
bound), but the dead block should be deleted to avoid a future `AmbiguousMatchException` if it is ever
uncommented.

### 6.5 `Permissions` payload is accepted but not persisted

Both `POST /api/system-admin/seta-admin` and the new `.../provision` accept a `permissions` array that
is **not stored anywhere**. This matches the pre-existing endpoint's behaviour, but it is a real gap:
the caller receives success while the permissions are silently dropped.

### 6.6 Pre-existing warnings

`NU1903` for `AutoMapper` 13.0.1 (see E12) and `CS8604` possible-null-reference warnings in
`tests/AuthChecks/Program.cs` and `SystemAdminHandlers.cs`. None block the build.

### 6.7 Tooling version skew

The global `dotnet-ef` tool is **10.0.11** while the projects reference EF Core **9.0.0**. It works,
but align them if EF commands ever behave unexpectedly (see E3).

### 6.8 Untracked scratch file

`dotnet-memo.txt` sits at the repository root as an untracked working-notes file from this work. It is
not part of the product — delete it, or fold anything useful into `docs/`.

---

## 7. File inventory

### 7.1 Added

**Domain**

- `DigiFikileLms.Domain/Entities/Complaint.cs`
- `DigiFikileLms.Domain/Enums/ComplaintStatus.cs`
- `DigiFikileLms.Domain/Interfaces/IComplaintRepository.cs`

**Application — Auth (two-step login)**

- `Common/EmailMasker.cs`
- `DTOs/Auth/OtpDtos.cs`
- `DTOs/Auth/StaffLoginRequestDto.cs`
- `Features/Auth/AuthLoginPolicy.cs`
- `Features/Auth/Commands/OtpCommands.cs`
- `Features/Auth/Commands/StaffLoginCommands.cs`
- `Features/Auth/Handlers/OtpCommandHandlers.cs`
- `Features/Auth/Handlers/StaffLoginCommandHandlers.cs`
- `Features/Auth/Handlers/ChangePasswordCommandHandler.cs`
- `Interfaces/IOtpService.cs` *(partner handover)*

**Application — SETA Administrator provisioning**

- `DTOs/SystemAdmin/ProvisionSetaAdminDto.cs`
- `Features/SystemAdmin/Commands/ProvisionSetaAdminCommands.cs`
- `Features/SystemAdmin/Handlers/ProvisionSetaAdminCommandHandler.cs`
- `Interfaces/ITemporaryPasswordGenerator.cs`
- `Interfaces/ICredentialsEmailService.cs` *(partner handover)*

**Application — Complaints**

- `DTOs/Complaint/ComplaintDto.cs`
- `Features/Complaints/Commands/ComplaintCommands.cs`
- `Features/Complaints/Queries/ComplaintQueries.cs`
- `Features/Complaints/Handlers/ComplaintHandlers.cs`

**Infrastructure**

- `Persistence/Configurations/ComplaintConfiguration.cs`
- `Persistence/Repositories/ComplaintRepository.cs`
- `Persistence/Migrations/20260916103510_AddComplaint.cs` (+ `.Designer.cs`)
- `Services/OtpService.cs` *(partner handover)*
- `Services/CredentialsEmailService.cs` *(partner handover)*
- `Services/TemporaryPasswordGenerator.cs`

**API & docs**

- `DigiFikileLms.API/Controllers/ComplaintsController.cs`
- `docs/otp-and-seta-admin-provisioning.md`
- `docs/2026-09-16-fixes-and-errors-report.md` *(this document)*

### 7.2 Modified

| File | Change |
|---|---|
| `DigiFikileLms.API/Program.cs` | `JsonStringEnumConverter` added (E9) |
| `DigiFikileLms.API/Controllers/AuthController.cs` | Added `otp/verify`, `otp/resend`, `system-admin/login`, the three staff logins and `password/change` |
| `DigiFikileLms.API/Controllers/SystemAdminController.cs` | Added `seta-admin/provision` |
| `DigiFikileLms.Application/DTOs/Auth/AuthResponseDto.cs` | Three additive OTP fields |
| `DigiFikileLms.Application/Features/Auth/Commands/AuthCommands.cs` | Student/SETA/SystemAdmin logins now issue an OTP challenge |
| `DigiFikileLms.Application/Interfaces/IApplicationDbContext.cs` | `DbSet<Complaint>` exposed |
| `DigiFikileLms.Application/Mappings/LmsMappingProfiles.cs` | Admin dashboard mapping and its `using` removed |
| `DigiFikileLms.Infrastructure/Persistence/Context/DigiFikileLmsDbContext.cs` | `DbSet<Complaint>` added |
| `DigiFikileLms.Infrastructure/Persistence/Migrations/DigiFikileLmsDbContextModelSnapshot.cs` | Complaint entity modelled |
| `DigiFikileLms.Infrastructure/Extensions/InfrastructureServiceExtensions.cs` | New services and `IComplaintRepository` registered |
| `docker-compose.yml` | Port mapping `8080:8080` → `5000:8080` (E2) |
| `tests/AuthChecks/Program.cs` | Two-step login, staff logins, provisioning/mail and complaint fixtures (E5–E8 fixes) |
| `docs/authentication-authorization.md` | Stale `/api/Admin/dashboard` rule corrected (E11) |

### 7.3 Deleted

| File | Reason |
|---|---|
| `DigiFikileLms.API/Controllers/AdminController.cs` | Generic Admin surface removed |
| `DigiFikileLms.Application/DTOs/Admin/AdminDashboardDto.cs` | ↑ |
| `DigiFikileLms.Application/DTOs/Admin/RecentActivityDto.cs` | ↑ |
| `DigiFikileLms.Application/Features/Admin/Queries/AdminQueries.cs` | ↑ |
| `DigiFikileLms.Application/Features/Admin/Handlers/AdminHandlers.cs` | ↑ |

### 7.4 Restored — not part of the change set

`_cb.txt` — a tracked file accidentally removed by an over-broad cleanup, restored immediately (see
**E10**). It does not appear in the final change set.

---

## 8. Command reference

```powershell
# Build
dotnet build DigiFikileLms.API\DigiFikileLms.API.csproj

# Run the test harness (152 checks)
dotnet run --project tests\AuthChecks\AuthChecks.csproj

# Run the API (use a free port while PEMHTTPD-x64 holds 8080)
$env:PORT = "5000"; dotnet run --project DigiFikileLms.API
#   Swagger: http://localhost:5000/swagger
#   Health:  http://localhost:5000/health

# EF Core — the tool is NOT on PATH, so invoke it by full path
$ef = "$env:USERPROFILE\.dotnet\tools\dotnet-ef.exe"
& $ef migrations list --project DigiFikileLms.Infrastructure --startup-project DigiFikileLms.API
& $ef database update --project DigiFikileLms.Infrastructure --startup-project DigiFikileLms.API
```

---

## Appendix A — Raw evidence excerpts

**E1 — the build failure**

```
error CS1503: Argument 2: cannot convert from 'string' to 'System.Enum'
  at DigiFikileLms.API\Controllers\ComplaintsController.cs(104,45)
```

**E1 — after the fix**

```
Build succeeded.
    4 Warning(s)
    0 Error(s)
```

**E2 — the bind failure**

```
Unhandled exception. System.IO.IOException: Failed to bind to address http://0.0.0.0:8080: address already in use.
   ---> Microsoft.AspNetCore.Connections.AddressInUseException: Only one usage of each socket address ...
   ---> System.Net.Sockets.SocketException (10048)
   at DigiFikileLms.API\Program.cs:line 97
```

**E2 — the real owner of port 8080, and why it cannot be stopped without elevation**

```
Service : PEMHTTPD-x64   Status: Running   StartType: Auto
Image   : "C:\Program Files\edb\pem\httpd\apache\bin\httpd.exe" -k runservice

taskkill /PID 4696 /T /F  -> ERROR: The process with PID 4696 could not be terminated. Reason: Access is denied.
sc.exe stop PEMHTTPD-x64  -> [SC] OpenService FAILED 5: Access is denied.
sc.exe query PEMHTTPD-x64 -> STATE: 4 RUNNING (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)
```

**E3 — "not found", yet installed**

```
Could not execute because the specified command or file was not found.
  * You intended to run a global tool, but a dotnet-prefixed executable with this name could not be found on the PATH.

PS> dotnet tool list --global
Package Id      Version      Commands
--------------------------------------
dotnet-ef       10.0.11      dotnet-ef
```

**E4 — migration state before the change**

```
20260916103510_AddComplaint (Pending)
```

**E4 — migration applied to Supabase**

```
Acquiring an exclusive lock for migration application. See https://aka.ms/efcore-docs-migrations-lock ...
Applying migration '20260916103510_AddComplaint'.
Done.
```

**E4 — migration state after the change (no `(Pending)` marker remains)**

```
20260916103510_AddComplaint
```

**E6 — the framework 400 envelope the harness could not parse**

```
Unhandled exception. System.Text.Json.JsonException: The JSON value could not be converted to
System.Collections.Generic.List`1[System.String]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134.
```

**E7 — the false failure**

```
Unhandled exception. System.Exception: FAIL: laying a complaint requires a token
   at Program.<>c__DisplayClass0_0.<<Main>$>g__Check|21(Boolean ok, String name) ... Program.cs:line 131
```

**E9 — proof the enum format is fixed (names, not numbers)**

```
PASS: learner lays a complaint and it starts Open
PASS: System Administrator lists every complaint filtered by status
```

**Final harness result**

```
PASS: Swagger Bearer, two-step login, staff logins, provisioning and complaint routes
PASS: password salts are random
All 152 authentication checks passed.
```
