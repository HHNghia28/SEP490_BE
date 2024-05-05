using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_API.Migrations
{
    /// <inheritdoc />
    public partial class createAccountAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "Address", "Avatar", "Birthday", "Email", "Fullname", "Gender", "IsBachelor", "IsDoctor", "IsMaster", "IsProfessor", "Nation", "Phone" },
                values: new object[] { new Guid("551b2cec-eaf0-4856-b892-f8ee4af4d3a3"), "600 Nguyễn Văn Cừ", "https://cantho.fpt.edu.vn/Data/Sites/1/media/logo-moi.png", new DateTime(2024, 5, 5, 10, 15, 3, 725, DateTimeKind.Local).AddTicks(7981), "admin@fpt.edu.vn", "Lê Văn Admin", "Nam", false, false, false, false, "Kinh", "0987654321" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "ID", "CreateAt", "CreateBy", "IsActive", "Password", "RefreshToken", "RefreshTokenExpires", "UpdateAt", "UpdateBy", "UserID", "Username" },
                values: new object[] { "GV0001", new DateTime(2024, 5, 5, 10, 15, 3, 725, DateTimeKind.Local).AddTicks(8261), "GV0001", true, "$2a$11$jp.Yl0PovlDZizEI9r8BIeBGr4pufkEmfekWlTn4W8ycMOSidTHee", "", new DateTime(2024, 5, 5, 10, 15, 3, 927, DateTimeKind.Local).AddTicks(2835), new DateTime(2024, 5, 5, 10, 15, 3, 725, DateTimeKind.Local).AddTicks(8262), "GV0001", new Guid("551b2cec-eaf0-4856-b892-f8ee4af4d3a3"), "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "ID",
                keyValue: "GV0001");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "ID",
                keyValue: new Guid("551b2cec-eaf0-4856-b892-f8ee4af4d3a3"));
        }
    }
}
