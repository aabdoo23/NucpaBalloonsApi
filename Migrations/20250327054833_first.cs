using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NucpaBalloonsApi.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContestId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodeforcesApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeforcesApiSecret = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminSettings_Admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CodeforcesId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemBalloonMaps",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminSettingsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProblemIndex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BalloonColor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemBalloonMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemBalloonMaps_AdminSettings_AdminSettingsId",
                        column: x => x.AdminSettingsId,
                        principalTable: "AdminSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BalloonRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubmissionId = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProblemIndex = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    BalloonColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PickedUpAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PickedUpBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalloonRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BalloonRequests_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminSettings_AdminId",
                table: "AdminSettings",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_BalloonRequests_TeamId",
                table: "BalloonRequests",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemBalloonMaps_AdminSettingsId",
                table: "ProblemBalloonMaps",
                column: "AdminSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_RoomId",
                table: "Teams",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalloonRequests");

            migrationBuilder.DropTable(
                name: "ProblemBalloonMaps");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "AdminSettings");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Admins");
        }
    }
}
