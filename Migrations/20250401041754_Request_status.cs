using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NucpaBalloonsApi.Migrations
{
    /// <inheritdoc />
    public partial class Request_status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "BalloonRequests");

            migrationBuilder.DropColumn(
                name: "DeliveredBy",
                table: "BalloonRequests");

            migrationBuilder.RenameColumn(
                name: "PickedUpBy",
                table: "BalloonRequests",
                newName: "StatusChangedBy");

            migrationBuilder.RenameColumn(
                name: "PickedUpAt",
                table: "BalloonRequests",
                newName: "StatusChangedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusChangedBy",
                table: "BalloonRequests",
                newName: "PickedUpBy");

            migrationBuilder.RenameColumn(
                name: "StatusChangedAt",
                table: "BalloonRequests",
                newName: "PickedUpAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "BalloonRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveredBy",
                table: "BalloonRequests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
