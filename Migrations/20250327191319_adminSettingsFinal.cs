using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NucpaBalloonsApi.Migrations
{
    /// <inheritdoc />
    public partial class adminSettingsFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "CodeforcesId",
                table: "Teams",
                newName: "CodeforcesHandle");

            migrationBuilder.AddColumn<string>(
                name: "AdminSettingsId",
                table: "Teams",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AdminSettingsId",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_AdminSettingsId",
                table: "Teams",
                column: "AdminSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AdminSettingsId",
                table: "Rooms",
                column: "AdminSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AdminSettings_AdminSettingsId",
                table: "Rooms",
                column: "AdminSettingsId",
                principalTable: "AdminSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AdminSettings_AdminSettingsId",
                table: "Teams",
                column: "AdminSettingsId",
                principalTable: "AdminSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AdminSettings_AdminSettingsId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AdminSettings_AdminSettingsId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_AdminSettingsId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_AdminSettingsId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "AdminSettingsId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "AdminSettingsId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "CodeforcesHandle",
                table: "Teams",
                newName: "CodeforcesId");

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
