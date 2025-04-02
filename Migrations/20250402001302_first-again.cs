using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NucpaBalloonsApi.Migrations
{
    /// <inheritdoc />
    public partial class firstagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContestId = table.Column<int>(type: "int", nullable: false),
                    CodeforcesApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeforcesApiSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSettings", x => x.Id);
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
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: true),
                    AdminSettingsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_AdminSettings_AdminSettingsId",
                        column: x => x.AdminSettingsId,
                        principalTable: "AdminSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CodeforcesHandle = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminSettingsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_AdminSettings_AdminSettingsId",
                        column: x => x.AdminSettingsId,
                        principalTable: "AdminSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teams_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "BalloonRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubmissionId = table.Column<long>(type: "bigint", nullable: false),
                    ContestId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProblemIndex = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    BalloonColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_BalloonRequests_SubmissionId",
                table: "BalloonRequests",
                column: "SubmissionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BalloonRequests_TeamId",
                table: "BalloonRequests",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemBalloonMaps_AdminSettingsId",
                table: "ProblemBalloonMaps",
                column: "AdminSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AdminSettingsId",
                table: "Rooms",
                column: "AdminSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_AdminSettingsId",
                table: "Teams",
                column: "AdminSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CodeforcesHandle",
                table: "Teams",
                column: "CodeforcesHandle",
                unique: true);

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
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "AdminSettings");
        }
    }
}
