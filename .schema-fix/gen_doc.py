# -*- coding: utf-8 -*-
"""Generate the DigiFikile LMS schema-alignment Word document."""
from docx import Document
from docx.shared import Pt, Inches, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT

doc = Document()
style = doc.styles["Normal"]
style.font.name = "Calibri"
style.font.size = Pt(10.5)

def heading(text, level=1):
    return doc.add_heading(text, level=level)

def para(text, bold=False, italic=False, size=None):
    p = doc.add_paragraph()
    r = p.add_run(text)
    r.bold = bold
    r.italic = italic
    if size:
        r.font.size = Pt(size)
    return p

def bullets(items):
    for it in items:
        doc.add_paragraph(it, style="List Bullet")

def numbered(items):
    for it in items:
        doc.add_paragraph(it, style="List Number")

def code(text):
    p = doc.add_paragraph()
    r = p.add_run(text)
    r.font.name = "Consolas"
    r.font.size = Pt(9.5)
    p.paragraph_format.left_indent = Inches(0.25)
    return p

def table(headers, rows, widths=None):
    t = doc.add_table(rows=1, cols=len(headers))
    t.style = "Light Grid Accent 1"
    t.alignment = WD_TABLE_ALIGNMENT.CENTER
    hdr = t.rows[0].cells
    for i, h in enumerate(headers):
        hdr[i].text = ""
        run = hdr[i].paragraphs[0].add_run(h)
        run.bold = True
    for row in rows:
        cells = t.add_row().cells
        for i, val in enumerate(row):
            cells[i].text = str(val)
    if widths:
        for i, w in enumerate(widths):
            for row in t.rows:
                row.cells[i].width = Inches(w)
    return t

# ---------- title ----------
title = doc.add_paragraph()
title.alignment = WD_ALIGN_PARAGRAPH.CENTER
tr = title.add_run("DigiFikile LMS Backend\nSchema Alignment with ERD")
tr.bold = True
tr.font.size = Pt(22)
sub = doc.add_paragraph()
sub.alignment = WD_ALIGN_PARAGRAPH.CENTER
sr = sub.add_run("Fixes and Changes, New Entities, API Reference & How to Run\n"
                 "Feature branch: feature/LMS-0022-Database-Api")
sr.font.size = Pt(12)
sr.font.color.rgb = RGBColor(0x40, 0x40, 0x40)
doc.add_paragraph()

# ---------- 1. Overview ----------
heading("1. Overview", 1)
para(
    "This document records all fixes and changes applied to align the Supabase (PostgreSQL) database "
    "schema with the ERD, and the supporting clean-architecture code (Domain, Application, Infrastructure, API). "
    "Key outcomes:"
)
bullets([
    "Renamed the Assessor role to Moderator and the Administrator role to SetaAdministrator, per the ERD.",
    "Added the Role and Permission entities plus the RolePermissions join table (many-to-many RBAC).",
    "UserAccounts now link to Roles (RoleId FK) instead of legacy role tables via shadow columns.",
    "Removed the legacy shadow foreign keys on UserAccounts (AdministratorId, AssessorId, FacilitatorId, TrainingProviderId).",
    "Renamed dependent FK columns: Feedbacks.AssessorId -> ModeratorId, Reports.AdministratorId -> SetaAdministratorId, "
    "SETAProgrammes.AdministratorId -> SetaAdministratorId.",
    "All one-to-one user-profile relationships are now paired (no shadow FK columns), each with a unique index on UserId.",
    "JWT is intentionally ignored for now - all endpoints are [AllowAnonymous] so they can be tested directly.",
])

# ---------- 2. Database changes ----------
heading("2. Database Migration: AlignSchemaWithErd (20260909210819)", 1)
para("A single hand-corrected EF Core migration was applied to the live Supabase database (schema lms).")

heading("2.1 Tables created", 2)
table(
    ["Table", "Columns", "Notes"],
    [
        ("Roles", "Id (PK), Name (unique), Description, CreatedAt, UpdatedAt", "New ERD entity"),
        ("Permissions", "Id (PK), Code (unique), Name, Description, CreatedAt, UpdatedAt", "New ERD entity"),
        ("RolePermissions", "RoleId + PermissionId (composite PK)", "Join table: Role <-> Permission"),
        ("Moderators", "Id (PK), UserId (FK, unique), StaffNumber, CreatedAt, UpdatedAt", "Renamed from Assessors"),
        ("SetaAdministrators", "Id (PK), UserId (FK, unique), CreatedAt, UpdatedAt", "Renamed from Administrators"),
    ],
    widths=[1.7, 2.6, 2.2],
)

heading("2.2 Columns renamed", 2)
table(
    ["Table", "Old column", "New column", "Reason"],
    [
        ("UserAccounts", "AssessorId", "SetaAdministratorId", "Assessor -> SetaAdministrator association"),
        ("UserAccounts", "AdministratorId", "RoleId", "RBAC via roles, per ERD"),
        ("Feedbacks", "AssessorId", "ModeratorId", "Assessor -> Moderator"),
        ("Reports", "AdministratorId", "SetaAdministratorId", "Administrator -> SetaAdministrator"),
        ("SETAProgrammes", "AdministratorId", "SetaAdministratorId", "Administrator -> SetaAdministrator"),
    ],
    widths=[1.5, 1.5, 1.7, 2.3],
)

heading("2.3 Legacy objects removed", 2)
bullets([
    "Dropped the Administrators and Assessors tables (replaced by SetaAdministrators / Moderators).",
    "Dropped legacy UserAccounts FKs and their indexes (AdministratorId, AssessorId, FacilitatorId, "
    "TrainingProviderId) - profiles now link through their own tables' unique UserId.",
    "Data preserved: columns were renamed in place (not drop/recreate), so existing rows survive.",
])

heading("2.4 Relationship fixes (shadow-FK bug)", 2)
para(
    "The original one-to-one configurations used unpaired WithOne(), which made EF create bogus one-to-many "
    "shadow FKs on UserAccounts (FacilitatorId, TrainingProviderId, ModeratorId, SetaAdministratorId). These were "
    "paired with the inverse navigation on UserAccount (u => u.Facilitator etc.) and unique indexes added, "
    "matching the Student pattern that was already correct."
)
table(
    ["Configuration file", "Fix applied"],
    [
        ("FacilitatorConfiguration.cs", "Paired WithOne(u => u.Facilitator) + cascade delete"),
        ("TrainingProviderConfiguration.cs", "Paired WithOne(u => u.TrainingProvider) + cascade delete"),
        ("ModeratorConfiguration.cs", "Paired WithOne(u => u.Moderator) + unique index on UserId"),
        ("SetaAdministratorConfiguration.cs", "Paired WithOne(u => u.SetaAdministrator) + unique index on UserId"),
        ("RoleConfiguration.cs", "Explicit RolePermissions join table (RoleId, PermissionId) with composite PK"),
    ],
    widths=[2.6, 3.9],
)

# ---------- 3. Clean-architecture code changes ----------
heading("3. Clean-Architecture Code Changes", 1)

heading("3.1 Domain layer (DigiFikileLms.Domain)", 2)
table(
    ["Item", "Type", "Location"],
    [
        ("Role, Permission", "New entities", "Entities/Role.cs, Entities/Permission.cs"),
        ("Moderator (from Assessor)", "Renamed entity", "Entities/Moderator.cs"),
        ("SetaAdministrator (from Administrator)", "Renamed entity", "Entities/SetaAdministrator.cs"),
        ("UserRole enum updated", "Modified", "Enums/UserRole.cs (Moderator, SetaAdministrator)"),
        ("IModeratorRepository, ISetaAdministratorRepository", "New interfaces", "Interfaces/"),
        ("IRoleRepository, IPermissionRepository", "New interfaces", "Interfaces/"),
        ("UserAccount role/profile navigations", "Modified", "Entities/UserAccount.cs"),
    ],
    widths=[2.9, 1.4, 2.2],
)

heading("3.2 Application layer (DigiFikileLms.Application)", 2)
bullets([
    "Features/Roles - Commands, Queries, Handlers, validators (CRUD + assign permission).",
    "Features/Permissions - Commands, Queries, Handlers, validators (CRUD).",
    "Features/Moderators - Commands, Queries, Handlers, validators (CRUD + lookup by user).",
    "Features/SetaAdministrators - Commands, Queries, Handlers, validators (CRUD + lookup by user).",
    "DTOs: Roles/RoleDtos.cs, Permissions, Moderators/ModeratorDto.cs, SetaAdministrators/SetaAdministratorDto.cs.",
    "Validators: RoleCommandValidators.cs, PermissionCommandValidators.cs, ModeratorCommandValidators.cs, "
    "SetaAdministratorCommandValidators.cs.",
    "Mappings/LmsMappingProfiles.cs - Role/Permission/Moderator/SetaAdministrator profiles registered with AutoMapper.",
    "Interfaces/IApplicationDbContext.cs - DbSet additions for Roles, Permissions, Moderators, SetaAdministrators.",
    "Features/Students/Handlers - completed missing handlers so the full Students controller works "
    "(GetAll, GetById, Update, Delete, enrollments/progress/certificates).",
])

heading("3.3 Infrastructure layer (DigiFikileLms.Infrastructure)", 2)
bullets([
    "Persistence/Configurations: RoleConfiguration, PermissionConfiguration, ModeratorConfiguration, "
    "SetaAdministratorConfiguration (plus fixes to Facilitator/TrainingProvider/UserAccount configurations).",
    "Persistence/Repositories: RoleRepository, PermissionRepository, ModeratorRepository, "
    "SetaAdministratorRepository (renamed from Assessor/Administrator repositories).",
    "Persistence/Migrations: 20260909210819_AlignSchemaWithErd (hand-corrected, applied to live DB).",
    "Extensions/InfrastructureServiceExtensions.cs - repository DI registrations updated.",
])

heading("3.4 API layer (DigiFikileLms.API)", 2)
bullets([
    "Controllers: RolesController, PermissionsController, ModeratorsController, SetaAdministratorsController (new); "
    "AdminController replaced by SetaAdministratorsController.",
    "All endpoints [AllowAnonymous] until JWT is reintroduced.",
    "Program.cs loads DotNetEnv (Env.TraversePath().Load()) so ConnectionStrings__DigiFikileLmsDb comes from .env.",
])

# ---------- 4. API reference ----------
heading("4. API Endpoint Reference", 1)
para("Base URL in development: http://localhost:5000 (or https://localhost:7001). Swagger UI at the root.")

table(
    ["Endpoint", "Method", "Purpose"],
    [
        ("api/roles", "GET", "List roles"),
        ("api/roles/{id}", "GET", "Get role by id"),
        ("api/roles", "POST", "Create role"),
        ("api/roles/{id}", "PUT", "Update role"),
        ("api/roles/{id}", "DELETE", "Delete role"),
        ("api/roles/{id}/permissions", "GET", "Permissions of a role"),
        ("api/roles/{id}/permissions/{permissionId}", "POST", "Assign permission to role"),
        ("api/permissions", "GET / POST", "List / create permissions"),
        ("api/permissions/{id}", "GET / PUT / DELETE", "Get / update / delete permission"),
        ("api/moderators", "GET / POST", "List / create moderators"),
        ("api/moderators/{id}", "GET / PUT / DELETE", "Get / update / delete moderator"),
        ("api/moderators/user/{userId}", "GET", "Moderator by user id"),
        ("api/setaadministrators", "GET / POST", "List / create SETA administrators"),
        ("api/setaadministrators/{id}", "GET / PUT / DELETE", "Get / update / delete SETA admin"),
        ("api/setaadministrators/user/{userId}", "GET", "SETA admin by user id"),
        ("api/students", "GET / POST", "List / create students"),
        ("api/students/{id}", "GET / PUT / DELETE", "Get / update / delete student"),
        ("api/students/profile/{userId}", "GET / PUT", "Get / update own profile"),
        ("api/students/{studentId}/enrollments|progress|certificates", "GET", "Student dashboards"),
        ("api/auth/student/register", "POST", "Student self-registration"),
        ("api/auth/student/login", "POST", "Student login"),
        ("api/auth/admin/login", "POST", "Staff (moderator / SETA admin) login"),
        ("api/assessments, api/enrollments, api/reports, api/users", "various", "Unchanged existing features"),
    ],
    widths=[2.7, 1.4, 2.9],
)

# ---------- 5. How to run ----------
heading("5. How to Run (including .env setup)", 1)

heading("5.1 Prerequisites", 2)
bullets([
    ".NET SDK 9.0 or later (check with: dotnet --version).",
    "EF Core tool (already installed here, v10.0.11): dotnet tool install --global dotnet-ef",
    "A Supabase project (PostgreSQL) containing the lms schema.",
])

heading("5.2 Configure environment variables from .env.example", 2)
numbered([
    "Copy the template at the repository root to a real .env file (PowerShell):",
])
code("Copy-Item .env.example .env")
numbered([
    "Open .env and replace the two placeholders:",
])
code("ConnectionStrings__DigiFikileLmsDb=Host=aws-1-eu-west-1.pooler.supabase.com;Port=5432;Database=postgres;"
     "Username=postgres.<project-ref>;Password=<database-password>;SslMode=Require;"
     "TrustServerCertificate=true;Pooling=true;Maximum Pool Size=100;SearchPath=lms;")
bullets([
    "<project-ref> - your Supabase project reference (Dashboard > Settings > General; it is the suffix of the "
    "pooler username, e.g. postgres.wclxndipmmxoemgrycda).",
    "<database-password> - the database password you set when creating the Supabase project.",
    "Keep .env secret; never commit it. Only .env.example belongs in source control.",
])
numbered([
    "The API loads this automatically: Program.cs calls DotNetEnv.Env.TraversePath().Load() and reads "
    "ConnectionStrings__DigiFikileLmsDb (double underscore = nested configuration key) as an environment variable. "
    "No appsettings edit is required.",
])

heading("5.3 Build and run", 2)
code("dotnet build DigiFikileLms.sln")
code("dotnet run --project DigiFikileLms.API")
para("Pending migrations are applied automatically at startup (DbInitializer) and the API listens on "
     "http://localhost:5000. Open the root URL for Swagger UI.")

heading("5.4 Apply migrations manually (optional)", 2)
code("dotnet ef database update --project DigiFikileLms.Infrastructure")
code("dotnet ef migrations list --project DigiFikileLms.Infrastructure")

heading("5.5 Smoke-test the new endpoints", 2)
code("Invoke-RestMethod http://localhost:5000/api/roles")
code("Invoke-RestMethod http://localhost:5000/api/permissions")
code("Invoke-RestMethod http://localhost:5000/api/moderators")
code("Invoke-RestMethod http://localhost:5000/api/setaadministrators")
code('Invoke-RestMethod "http://localhost:5000/api/students?page=1&pageSize=5"')
para("Example create-role body (POST api/roles):", bold=True)
code('{ "name": "Moderator", "description": "Moderates results and gives feedback" }')
para("Example create-permission body (POST api/permissions):", bold=True)
code('{ "code": "students.read", "name": "Read students", "description": "View student records" }')

# ---------- 6. Verification ----------
heading("6. Verification Performed", 1)
bullets([
    "dotnet build DigiFikileLms.sln - Build succeeded, 0 errors.",
    "dotnet ef database update - applied AlignSchemaWithErd to the live Supabase lms schema.",
    "Live schema dump verified: Roles, Permissions, RolePermissions, Moderators, SetaAdministrators exist; "
    "legacy Administrators/Assessors tables and shadow FK columns are gone.",
    "Runtime smoke tests: GET api/roles, api/permissions, api/moderators, api/setaadministrators and "
    "api/students all returned success after the student-handler fix.",
    "Student feature handlers completed (GetAll, GetById, Update, Delete, enrollments/progress/certificates) "
    "so the whole Students controller works against the new schema.",
])

# ---------- 7. Notes ----------
heading("7. Notes and Next Steps", 1)
bullets([
    "JWT is intentionally disabled ([AllowAnonymous] everywhere); when re-enabled, build authorization policies "
    "from the new Role/Permission tables.",
    "Package advisory: AutoMapper 13.0.1 is flagged (NU1903, high severity) - consider upgrading.",
    "The Submission.StudentId1 shadow-FK warning comes from a pre-existing duplicate relationship in the "
    "Submission configuration - harmless today, but worth pairing like the user-profile fixes above.",
])

doc.save("DigiFikileLms-Backend-Change-Documentation.docx")
print("Document saved: DigiFikileLms-Backend-Change-Documentation.docx")



