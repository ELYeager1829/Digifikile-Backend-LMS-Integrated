using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DigiFikileLms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlignSchemaWithErd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Assessors_AssessorId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Administrators_AdministratorId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_SETAProgrammes_Administrators_AdministratorId",
                table: "SETAProgrammes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_Administrators_AdministratorId",
                table: "UserAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_Assessors_AssessorId",
                table: "UserAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_Facilitators_FacilitatorId",
                table: "UserAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_TrainingProviders_TrainingProviderId",
                table: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "Assessors");

            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_TrainingProviderId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "AdministratorId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "AssessorId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "FacilitatorId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "TrainingProviderId",
                table: "UserAccounts");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "UserAccounts",
                type: "integer",
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "AdministratorId",
                table: "SETAProgrammes",
                newName: "SetaAdministratorId");

            migrationBuilder.RenameIndex(
                name: "IX_SETAProgrammes_AdministratorId",
                table: "SETAProgrammes",
                newName: "IX_SETAProgrammes_SetaAdministratorId");

            migrationBuilder.RenameColumn(
                name: "AdministratorId",
                table: "Reports",
                newName: "SetaAdministratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_AdministratorId",
                table: "Reports",
                newName: "IX_Reports_SetaAdministratorId");

            migrationBuilder.RenameColumn(
                name: "AssessorId",
                table: "Feedbacks",
                newName: "ModeratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_AssessorId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_ModeratorId");

            migrationBuilder.CreateTable(
                name: "Moderators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    StaffNumber = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moderators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Moderators_UserAccounts_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SetaAdministrators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetaAdministrators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetaAdministrators_UserAccounts_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_RoleId",
                table: "UserAccounts",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Moderators_UserId",
                table: "Moderators",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SetaAdministrators_UserId",
                table: "SetaAdministrators",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Moderators_ModeratorId",
                table: "Feedbacks",
                column: "ModeratorId",
                principalTable: "Moderators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_SetaAdministrators_SetaAdministratorId",
                table: "Reports",
                column: "SetaAdministratorId",
                principalTable: "SetaAdministrators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SETAProgrammes_SetaAdministrators_SetaAdministratorId",
                table: "SETAProgrammes",
                column: "SetaAdministratorId",
                principalTable: "SetaAdministrators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_Roles_RoleId",
                table: "UserAccounts",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Moderators_ModeratorId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_SetaAdministrators_SetaAdministratorId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_SETAProgrammes_SetaAdministrators_SetaAdministratorId",
                table: "SETAProgrammes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_Roles_RoleId",
                table: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Moderators");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SetaAdministrators");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_RoleId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UserAccounts");

            migrationBuilder.RenameColumn(
                name: "SetaAdministratorId",
                table: "SETAProgrammes",
                newName: "AdministratorId");

            migrationBuilder.RenameIndex(
                name: "IX_SETAProgrammes_SetaAdministratorId",
                table: "SETAProgrammes",
                newName: "IX_SETAProgrammes_AdministratorId");

            migrationBuilder.RenameColumn(
                name: "SetaAdministratorId",
                table: "Reports",
                newName: "AdministratorId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_SetaAdministratorId",
                table: "Reports",
                newName: "IX_Reports_AdministratorId");

            migrationBuilder.RenameColumn(
                name: "ModeratorId",
                table: "Feedbacks",
                newName: "AssessorId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_ModeratorId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_AssessorId");

            migrationBuilder.AddColumn<int>(
                name: "AdministratorId",
                table: "UserAccounts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssessorId",
                table: "UserAccounts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacilitatorId",
                table: "UserAccounts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingProviderId",
                table: "UserAccounts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administrators_UserAccounts_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assessors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StaffNumber = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assessors_UserAccounts_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_AdministratorId",
                table: "UserAccounts",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_AssessorId",
                table: "UserAccounts",
                column: "AssessorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_FacilitatorId",
                table: "UserAccounts",
                column: "FacilitatorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_TrainingProviderId",
                table: "UserAccounts",
                column: "TrainingProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_UserId",
                table: "Administrators",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assessors_UserId",
                table: "Assessors",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Assessors_AssessorId",
                table: "Feedbacks",
                column: "AssessorId",
                principalTable: "Assessors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Administrators_AdministratorId",
                table: "Reports",
                column: "AdministratorId",
                principalTable: "Administrators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SETAProgrammes_Administrators_AdministratorId",
                table: "SETAProgrammes",
                column: "AdministratorId",
                principalTable: "Administrators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_Administrators_AdministratorId",
                table: "UserAccounts",
                column: "AdministratorId",
                principalTable: "Administrators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_Assessors_AssessorId",
                table: "UserAccounts",
                column: "AssessorId",
                principalTable: "Assessors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_Facilitators_FacilitatorId",
                table: "UserAccounts",
                column: "FacilitatorId",
                principalTable: "Facilitators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_TrainingProviders_TrainingProviderId",
                table: "UserAccounts",
                column: "TrainingProviderId",
                principalTable: "TrainingProviders",
                principalColumn: "Id");
        }
    }
}
