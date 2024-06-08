using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_API.Migrations
{
    /// <inheritdoc />
    public partial class updateStudentByStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionID", "RoleID" },
                values: new object[] { 7, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionID", "RoleID" },
                keyValues: new object[] { 7, 2 });

        }
    }
}
