# Incident & Fix Report — DigiFikile LMS Backend

**Date:** 2026-09-11
**Project:** DigiFikile-LMS.Backend
**Branch:** `feature/LMS-0022-Database-Api`
**Scope:** Two incidents investigated and resolved in one session:

1. Supabase project reporting **unhealthy** (`schema "public" does not exist`)
2. API crashing on startup (`PendingModelChangesWarning` + EF Core shadow foreign key warning `Submission.StudentId1`)

Both incidents share the same underlying theme: **drift between what the platform/model expects and what the database actually contains.**

---

## 1. Incident A — Supabase project unhealthy

### 1.1 Symptoms

- The Supabase dashboard project status showed **unhealthy**.
- PostgREST API logs (exported as `supabase_logs (1).csv`) showed a repeating cycle every ~32 seconds:

  ```
  Attempting to reconnect to the database in 32 seconds...
  Successfully connected to PostgreSQL 17.6 on x86_64-pc-linux-gnu...
  Connection Pool initialized with a maximum size of 10 connections
  Config reloaded
  Failed to load the schema cache using db-schemas=graphql_public,lms and
  db-extra-search-path=public,extensions.
  {"code":"3F000","details":null,"hint":null,"message":"schema \"public\" does not exist"}
  ```

- Health checks from the management plane returned **HTTP 503**:

  ```
  127.0.0.1 - service_role [10/Sep/2026:11:13:38 +0000] "GET / HTTP/1.1" 503 119 ""
  "@supabase-infra/mgmt-api/v1.270.3"
  ```

- Postgres-side logs (`supabase_logs.csv`) showed the same failure from the database's point of
  view — one `3F000` error every ~32 seconds from at least 10:18 to 11:11 UTC (~97 occurrences):
  the database rejecting PostgREST's schema-cache load, not a connection problem.

### 1.2 Diagnosis

| Check | Result |
|---|---|
| Database connectivity | ✅ Fine — PostgREST connected to PostgreSQL 17.6 successfully on every retry |
| Schema cache load | ❌ Failed with `3F000` — `schema "public" does not exist` |
| Config | API exposed schemas `graphql_public,lms` with extra search path `public,extensions` |

**Root cause:** the `public` schema did not exist in the database while PostgREST's
configuration still referenced it. PostgREST could not build its schema cache, every health
check (`GET /`) returned 503, so the project reported unhealthy in a reconnect loop.

Likely origin of the drift: schema cleanup work on the `lms` schema (see `.schema-fix`
documentation and migration `AlignSchemaWithErd`) which removed/replaced the default schema
setup without updating Supabase's API exposure settings.

**Key fact:** the error is about *exposing* a non-existent schema — not about the `lms` schema,
which was present and serving the .NET application fine via direct Npgsql connections.

### 1.3 Fix applied

Removed `public` from the Supabase API exposure (Dashboard → **Project Settings → API →
Exposed schemas**), leaving only the schemas that actually exist and are used:

```
graphql_public, lms
```

Alternatives considered (documented for future reference):

| Option | Verdict |
|---|---|
| Remove `public` from exposed schemas *(chosen)* | Clean — matches reality (no `public` schema, REST API serves `lms`) |
| Recreate an empty `public` schema + default grants | Also valid (Supabase default state), kept as fallback |
| Recreate `public` and re-expose it | Rejected — would expose an empty schema and reintroduce confusion |

### 1.4 Verification

- PostgREST recovered on its next reconnect cycle (~32 s); the `3F000` and `503` entries
  stopped in both log sources at ~11:11 UTC.
- Project status returned to **healthy**.
- No re-occurrence in later log exports during this session.
- Confirmed safe to keep `public` absent: the .NET backend uses the `lms` schema directly and
  nothing consumes REST endpoints under `public`.

### 1.5 Residual notes

- `db-extra-search-path=public,extensions` is part of the platform PostgREST configuration and
  is not editable from the dashboard for hosted projects; by itself it does not fail health
  checks once `public` is not in the exposed schemas list.
- If the error ever returns after a future config change, the immediate fallback is
  `CREATE SCHEMA public AUTHORIZATION postgres;` (empty schema, default grants) — this
  restores Supabase's default state and clears the error even if the exposure setting is
  reverted by accident.

---

## 2. Incident B — API crashes on startup (EF Core)

### 2.1 Symptoms

Running `dotnet run --project DigiFikileLms.Api` crashed at `Program.cs:48`
(`await dbContext.Database.MigrateAsync();`) with:

```
Unhandled exception. System.InvalidOperationException:
An error was generated for warning
'Microsoft.EntityFrameworkCore.Migrations.PendingModelChangesWarning':
The model for context 'DigiFikileLmsDbContext' has pending changes.
Add a new migration before updating the database.
```

In addition, this warning appeared during startup model validation (not fatal, but a smell):

```
warn: Microsoft.EntityFrameworkCore.Model.Validation[10625]
      The foreign key property 'Submission.StudentId1' was created in shadow state because a
      conflicting property with the simple name 'StudentId' exists in the entity type, but is
      either not mapped, is already used for another relationship, or is incompatible with the
      associated primary key type.
```

### 2.2 Diagnosis

Two distinct problems, both confirmed with EF Core tooling
(`dotnet ef migrations has-pending-model-changes`, snapshot inspection, `git diff`):

#### Problem 1 — Duplicate relationship on `Submission ↔ Student` (shadow FK)

`Student` has a collection navigation:

```csharp
// DigiFikileLms.Domain/Entities/Student.cs
public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();
```

…but the configuration left the relationship unpaired:

```csharp
// DigiFikileLms.Infrastructure/Persistence/Configurations/SubmissionConfiguration.cs (before)
builder.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId);
```

**Mechanism of the bug:**

1. EF Core's conventions discover `Submission.Student` ↔ `Student.Submissions` and build one
   relationship.
2. The explicit configuration then adds a **second** relationship over the same navigation with
   a bare `WithMany()` (meaning "no inverse navigation").
3. Both relationships want the `StudentId` column; only one can have it. EF Core therefore
   materialised the other FK as a **shadow property** `StudentId1` (nullable `int?`) with its
   own index and FK on the `Submissions` table — exactly what the migration snapshot
   (`DigiFikileLmsDbContextModelSnapshot.cs`) contained.

**Audit of the same pattern elsewhere** (all configuration files scanned for bare
`.WithMany()` / `.WithOne()`):

| File | Line(s) | Config | Verdict |
|---|---|---|---|
| `SubmissionConfiguration.cs` | 14 | `.WithMany()` on `Submission.Student` | ❌ Duplicate — `Student.Submissions` exists → shadow FK |
| `RoleConfiguration.cs` | 21/26/31 | many-to-many via `RolePermissions` join | ✅ Correct unidirectional join-table mapping |
| `GroupConfiguration.cs` | 27/32 | `Group.setaAdministrator` / `Group.setaProgramme` | ✅ No inverse collection exists on those entities |
| `GroupEnrollmentConfiguration.cs` | 30 | `GroupEnrollment.Student` | ✅ `Student` has no `GroupEnrollments` collection |
| `UserAccountConfiguration.cs` | 22 | `UserAccount.Role` | ✅ `Role` has no `UserAccounts` collection |

So `Submission` was the **only** offender.

#### Problem 2 — Pending model changes (the actual crash)

`dotnet ef migrations has-pending-model-changes` returned:

```
Changes have been made to the model since the last migration. Add a new migration.
```

Root cause: commits `96aabf0` ("Fixed some entities and have new endpoints") and `e85070c` /
`bf90174` (SystemAdmin API) added **new entities** to the model:

- `SystemAdministrator`
- `SystemLog`
- `Group`
- `GroupEnrollment`

…**without generating a migration**. The last migration on record was
`20260909210819_AlignSchemaWithErd`, so the live database was missing the 4 corresponding
tables, and the model no longer matched the snapshot — which .NET 9 surfaces as a hard
`PendingModelChangesWarning` exception during `MigrateAsync()`.

Ruled out as causes:

- Working-tree changes in `DigiFikileLms.Domain/Entities/Result.cs` and
  `UserAccountRepository.cs` — inspected via `git diff`; **comments only**, no model impact.
- AutoMapper `NU1903` warnings — package-audit notices only, unrelated to the runtime.

### 2.3 Fixes applied

#### Fix 1 — Pair the relationship (source fix)

```csharp
// DigiFikileLms.Infrastructure/Persistence/Configurations/SubmissionConfiguration.cs (after)
builder.HasOne(x => x.Student).WithMany(s => s.Submissions).HasForeignKey(x => x.StudentId);
```

This makes convention and configuration agree on **one** relationship keyed by `StudentId`,
and removes the shadow `StudentId1` property, its index and its FK from the model.

#### Fix 2 — New migration capturing the whole current model

```
dotnet ef migrations add FixSubmissionStudentRelationship `
  --project DigiFikileLms.Infrastructure --startup-project DigiFikileLms.API
```

Generated `20260911064643_FixSubmissionStudentRelationship.cs`. EF scaffolded it with a
"data loss possible" advisory, so `Up()` was **reviewed line-by-line before applying**:

| Operation | Detail | Risk assessment |
|---|---|---|
| `DropForeignKey` `FK_Submissions_Students_StudentId1` | removes the duplicate relationship's FK | safe — shadow FK, unused by application code |
| `DropIndex` `IX_Submissions_StudentId1` | its index | safe |
| `DropColumn` `Submissions.StudentId1` | the shadow column itself (nullable, written by nothing) | safe — only "data-loss class" op; column held no meaningful data |
| `CreateTable` `Groups` | FKs: `setaAdministratorId` → `SetaAdministrators` (Restrict), `setaProgrammeId` → `SETAProgrammes` (SetNull) | matches `GroupConfiguration` |
| `CreateTable` `SystemAdministrators` | FK `UserId` → `UserAccounts` (Cascade), unique index on `UserId` | matches entity |
| `CreateTable` `GroupEnrollments` | FKs to `Groups`/`Students` (Cascade), unique index `(GroupId, StudentId)` | matches `GroupEnrollmentConfiguration` |
| `CreateTable` `SystemLogs` | FK `SystemAdministratorId` → `SystemAdministrators` (Cascade); indexes on `Action`, `CreatedAt`, `ResourceType`, `SystemAdministratorId` | matches `SystemLogConfiguration` |
| Index creations | as per configurations | safe |

`Down()` reverses everything (drops the 4 tables, re-adds the shadow column/FK/index), so the
migration is fully reversible.

**Note:** creating the missing tables is the *intended* outcome — the SystemAdmin endpoints
added in recent commits query `SystemAdministrators`/`SystemLogs`, and those tables did not
exist in the database until this migration ran.

#### Fix 3 — Documentation hygiene

`README.md` "Known remaining work" updated: the EF Core `Submission.StudentId1` shadow FK is
listed as **resolved**; only the AutoMapper `NU1903` advisory remains as a known warning.

### 2.4 Verification

| Check | Result |
|---|---|
| `dotnet build DigiFikileLms.sln` | ✅ Build succeeded — **0 errors**, 4 warnings (all AutoMapper `NU1903`) |
| Startup model validation | ✅ **No** `Submission.StudentId1` shadow-FK warning anymore |
| `dotnet ef migrations has-pending-model-changes` | ✅ *"No changes have been made to the model since the last migration."* |
| `dotnet run --project DigiFikileLms.Api` | ✅ Migration `FixSubmissionStudentRelationship` applied (`INSERT INTO "__EFMigrationsHistory" ... '9.0.0'`), `✅ Database migrated successfully!`, app listening on `https://localhost:7001` and `http://localhost:5000` |
| Reversibility | ✅ `Down()` drops the created tables and restores the old shadow-FK shape if ever needed |

### 2.5 Files changed

| File | Change |
|---|---|
| `DigiFikileLms.Infrastructure/Persistence/Configurations/SubmissionConfiguration.cs` | paired `WithMany(s => s.Submissions)` |
| `DigiFikileLms.Infrastructure/Persistence/Migrations/20260911064643_FixSubmissionStudentRelationship.cs` | new migration (EF-generated, manually reviewed) |
| `DigiFikileLms.Infrastructure/Persistence/Migrations/20260911064643_FixSubmissionStudentRelationship.Designer.cs` | generated |
| `DigiFikileLms.Infrastructure/Persistence/Migrations/DigiFikileLmsDbContextModelSnapshot.cs` | regenerated to match current model |
| `README.md` | known-issues line updated |
| `docs/2026-09-11-incident-and-fix-report.md` | this document |

---

## 3. Known remaining warnings (not fixed, documented)

| Warning | Meaning | Suggested action |
|---|---|---|
| `NU1903: AutoMapper 13.0.1 has a known high severity vulnerability (GHSA-rvv3-g6hj-g44x)` | NuGet package-audit advisory. Does **not** block the build; appears in the IDE Error List, which makes it look like an error | Upgrade the `AutoMapper` package reference in `DigiFikileLms.Application.csproj` and `DigiFikileLms.Infrastructure.csproj` to the patched version when convenient; verify mapping profiles still compile afterwards |

---

## 4. Process lessons / how to prevent recurrence

1. **Every entity/config change needs a migration.** .NET 9's `PendingModelChangesWarning` is
   the guard: when `MigrateAsync()` throws it, the model and migrations are out of sync.
   Workflow after touching entities or configurations:
   ```powershell
   dotnet ef migrations has-pending-model-changes --project DigiFikileLms.Infrastructure --startup-project DigiFikileLms.API
   dotnet ef migrations add <Name> --project DigiFikileLms.Infrastructure --startup-project DigiFikileLms.API
   # review the generated Up()/Down() BEFORE running the app (startup applies migrations automatically)
   ```
2. **Always pair `WithOne()`/`WithMany()`** with the actual inverse navigation when one exists.
   A bare `WithMany()` is only correct when the principal genuinely has no collection
   (unidirectional relationship). The tell-tale symptom is a `...Id1` shadow-FK warning at
   startup.
3. **Keep Supabase exposure settings in sync with the schema.** Removing a schema that
   PostgREST is configured to expose makes the whole project unhealthy — and the platform
   logs (both PostgREST and Postgres sources) are the fastest way to see the exact mismatch.
4. **Prefer the Postgres log source for platform monitoring** — in this incident it captured
   the failure for ~1 hour with a clean `3F000` signature.

---

## Appendix A — Key evidence excerpts

**PostgREST failure (Incident A):**

```
Failed to load the schema cache using db-schemas=graphql_public,lms and
db-extra-search-path=public,extensions.
{"code":"3F000","details":null,"hint":null,"message":"schema \"public\" does not exist"}
```

**Health checks (Incident A):**

```
127.0.0.1 - service_role [10/Sep/2026:11:13:38 +0000] "GET / HTTP/1.1" 503 119 ""
"@supabase-infra/mgmt-api/v1.270.3"
```

**Shadow FK snapshot evidence (Incident B, before fix):**
`DigiFikileLmsDbContextModelSnapshot.cs` contained on `Submission`:

```
b.Property<int>("StudentId")   ... b.HasIndex("StudentId");
b.Property<int?>("StudentId1") ... b.HasIndex("StudentId1");
... .HasForeignKey("StudentId1")
```

**Startup crash (Incident B):**

```
System.InvalidOperationException: An error was generated for warning
'Microsoft.EntityFrameworkCore.Migrations.PendingModelChangesWarning':
The model for context 'DigiFikileLmsDbContext' has pending changes.
   at Program.<Main>$(String[] args) in ...\DigiFikileLms.Api\Program.cs:line 48
```

**Successful recovery (Incident B):**

```
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260911064643_FixSubmissionStudentRelationship', '9.0.0');
✅ Database migrated successfully!
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:5000
```
