using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_API.Migrations
{
    /// <inheritdoc />
    public partial class updateDateNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Notifications");

        }
    }
}
