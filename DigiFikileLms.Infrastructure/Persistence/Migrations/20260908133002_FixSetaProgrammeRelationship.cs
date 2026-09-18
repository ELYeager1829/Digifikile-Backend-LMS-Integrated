using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigiFikileLms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixSetaProgrammeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SETAProgrammes_Administrators_AdministratorId1",
                table: "SETAProgrammes");

            migrationBuilder.DropIndex(
                name: "IX_SETAProgrammes_AdministratorId1",
                table: "SETAProgrammes");

            migrationBuilder.DropColumn(
                name: "AdministratorId1",
                table: "SETAProgrammes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdministratorId1",
                table: "SETAProgrammes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SETAProgrammes_AdministratorId1",
                table: "SETAProgrammes",
                column: "AdministratorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SETAProgrammes_Administrators_AdministratorId1",
                table: "SETAProgrammes",
                column: "AdministratorId1",
                principalTable: "Administrators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
