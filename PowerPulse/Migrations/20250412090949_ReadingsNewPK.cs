using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PowerPulse.Migrations
{
    /// <inheritdoc />
    public partial class ReadingsNewPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_meter_readings",
                table: "meter_readings");

            migrationBuilder.DropIndex(
                name: "IX_meter_readings_UserId",
                table: "meter_readings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "meter_readings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_meter_readings",
                table: "meter_readings",
                columns: new[] { "UserId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_meter_readings",
                table: "meter_readings");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "meter_readings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_meter_readings",
                table: "meter_readings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_meter_readings_UserId",
                table: "meter_readings",
                column: "UserId");
        }
    }
}
