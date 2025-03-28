using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NucpaBalloonsApi.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminSettings_Admins_AdminId",
                table: "AdminSettings");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropIndex(
                name: "IX_AdminSettings_AdminId",
                table: "AdminSettings");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "AdminSettings");

            migrationBuilder.AddColumn<string>(
                name: "AdminUsername",
                table: "AdminSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminUsername",
                table: "AdminSettings");

            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "AdminSettings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminSettings_AdminId",
                table: "AdminSettings",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminSettings_Admins_AdminId",
                table: "AdminSettings",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
