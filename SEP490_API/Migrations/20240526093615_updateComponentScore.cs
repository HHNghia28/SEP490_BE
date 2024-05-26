using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_API.Migrations
{
    /// <inheritdoc />
    public partial class updateComponentScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentScores_ComponentScores_ComponentScoreID",
                table: "StudentScores");

            migrationBuilder.DropIndex(
                name: "IX_StudentScores_ComponentScoreID",
                table: "StudentScores");

            

            migrationBuilder.DropColumn(
                name: "ComponentScoreID",
                table: "StudentScores");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "StudentScores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StudentScores",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ScoreFactor",
                table: "StudentScores",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Semester",
                table: "StudentScores",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
