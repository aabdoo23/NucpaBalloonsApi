using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NucpaBalloonsApi.Migrations
{
    /// <inheritdoc />
    public partial class team_problem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CodeforcesHandle",
                table: "Teams",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CodeforcesHandle",
                table: "Teams",
                column: "CodeforcesHandle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BalloonRequests_SubmissionId",
                table: "BalloonRequests",
                column: "SubmissionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_CodeforcesHandle",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_BalloonRequests_SubmissionId",
                table: "BalloonRequests");

            migrationBuilder.AlterColumn<string>(
                name: "CodeforcesHandle",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
