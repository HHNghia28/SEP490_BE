using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SEP490_API.Migrations
{
    /// <inheritdoc />
    public partial class initDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SchoolYears",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolYears", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Birthday = table.Column<DateTime>(type: "date", nullable: true),
                    Birthplace = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsMartyrs = table.Column<bool>(type: "bit", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    HomeTown = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FatherFullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FatherProfession = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FatherPhone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MotherFullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MotherProfession = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MotherPhone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBachelor = table.Column<bool>(type: "bit", nullable: true),
                    IsMaster = table.Column<bool>(type: "bit", nullable: true),
                    IsDoctor = table.Column<bool>(type: "bit", nullable: true),
                    IsProfessor = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    PermissionID = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.PermissionID, x.RoleID });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "Permissions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountStudents",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RefreshTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStudents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountStudents_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountStudents_Students_UserID",
                        column: x => x.UserID,
                        principalTable: "Students",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ComponentScores",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ScoreFactor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SubjectID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ComponentScores_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonsPlans",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Slot = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SubjectID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonsPlans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LessonsPlans_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RefreshTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentScores",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentScoreID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SchoolYearID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StudentScores_AccountStudents_StudentID",
                        column: x => x.StudentID,
                        principalTable: "AccountStudents",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentScores_ComponentScores_ComponentScoreID",
                        column: x => x.ComponentScoreID,
                        principalTable: "ComponentScores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentScores_SchoolYears_SchoolYearID",
                        column: x => x.SchoolYearID,
                        principalTable: "SchoolYears",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountPermissions",
                columns: table => new
                {
                    PermissionID = table.Column<int>(type: "int", nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPermissions", x => new { x.PermissionID, x.AccountID });
                    table.ForeignKey(
                        name: "FK_AccountPermissions_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountPermissions_Permissions_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "Permissions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountRoles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRoles", x => new { x.RoleID, x.AccountID });
                    table.ForeignKey(
                        name: "FK_AccountRoles_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountRoles_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountID = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherID = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Classroom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchoolYearID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Classes_Accounts_TeacherID",
                        column: x => x.TeacherID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Classes_SchoolYears_SchoolYearID",
                        column: x => x.SchoolYearID,
                        principalTable: "SchoolYears",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Notifications_Accounts_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SchoolAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SchoolPhone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SchoolEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SchoolLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolSettings_Accounts_CreateBy",
                        column: x => x.CreateBy,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherID = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    SlotByDate = table.Column<int>(type: "int", nullable: false),
                    SlotByLessonPlans = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Schedules_Accounts_TeacherID",
                        column: x => x.TeacherID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schedules_Classes_ClassID",
                        column: x => x.ClassID,
                        principalTable: "Classes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Schedules_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "StudentClasses",
                columns: table => new
                {
                    ClassID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentID = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClasses", x => new { x.StudentID, x.ClassID });
                    table.ForeignKey(
                        name: "FK_StudentClasses_AccountStudents_StudentID",
                        column: x => x.StudentID,
                        principalTable: "AccountStudents",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentClasses_Classes_ClassID",
                        column: x => x.ClassID,
                        principalTable: "Classes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Present = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Attendances_AccountStudents_StudentID",
                        column: x => x.StudentID,
                        principalTable: "AccountStudents",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_Schedules_ScheduleID",
                        column: x => x.ScheduleID,
                        principalTable: "Schedules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Add Teacher" },
                    { 3, "Update Teacher" },
                    { 4, "Delete Teacher" },
                    { 5, "Get Teacher" },
                    { 6, "Add Student" },
                    { 7, "Update Student" },
                    { 8, "Delete Student" },
                    { 9, "Get Student" },
                    { 10, "Add Subject" },
                    { 11, "Update Subject" },
                    { 12, "Delete Subject" },
                    { 13, "Get Subject" },
                    { 14, "Add Class" },
                    { 15, "Update Class" },
                    { 16, "Delete Class" },
                    { 17, "Get Class" },
                    { 18, "Add Schedule" },
                    { 19, "Update Schedule" },
                    { 20, "Delete Schedule" },
                    { 21, "Get Schedule" },
                    { 22, "Add Register Book" },
                    { 23, "Update Register Book" },
                    { 24, "Delete Register Book" },
                    { 25, "Get Register Book" },
                    { 26, "Add Attendance" },
                    { 27, "Update Attendance" },
                    { 28, "Delete Attendance" },
                    { 29, "Get Attendance" },
                    { 30, "Add Mark" },
                    { 31, "Update Mark" },
                    { 32, "Delete Mark" },
                    { 33, "Get Mark" },
                    { 34, "Add Notification" },
                    { 35, "Update Notification" },
                    { 36, "Delete Notification" },
                    { 37, "Get Notification" },
                    { 38, "Update Setting" },
                    { 39, "Get Log" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Student" },
                    { 3, "Homeroom Teacher" },
                    { 4, "Subject Teacher" },
                    { 5, "Supervisor" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "Address", "Avatar", "Birthday", "Email", "Fullname", "Gender", "IsBachelor", "IsDoctor", "IsMaster", "IsProfessor", "Nation", "Phone" },
                values: new object[,]
                {
                    { new Guid("033f2b95-93aa-46cc-94f2-89a74a865175"), "600 Nguyễn Văn Cừ", "https://cantho.fpt.edu.vn/Data/Sites/1/media/logo-moi.png", new DateTime(2024, 5, 13, 22, 55, 27, 954, DateTimeKind.Local).AddTicks(5652), "admin@fpt.edu.vn", "Lê Văn Admin", "Nam", false, false, false, false, "Kinh", "0987654321" },
                    { new Guid("2cef2fe7-0047-4811-ab43-e608103b3ec3"), "600 Nguyễn Văn Cừ", "https://cantho.fpt.edu.vn/Data/Sites/1/media/logo-moi.png", new DateTime(2024, 5, 13, 22, 55, 27, 612, DateTimeKind.Local).AddTicks(7212), "admin@fpt.edu.vn", "Lê Văn Admin", "Nam", false, false, false, false, "Kinh", "0987654321" },
                    { new Guid("3d63767e-e98f-490c-8e34-d7c65fb3aacc"), "600 Nguyễn Văn Cừ", "https://cantho.fpt.edu.vn/Data/Sites/1/media/logo-moi.png", new DateTime(2024, 5, 13, 22, 55, 27, 783, DateTimeKind.Local).AddTicks(5108), "admin@fpt.edu.vn", "Lê Văn Admin", "Nam", false, false, false, false, "Kinh", "0987654321" },
                    { new Guid("f64b2a6a-ee14-4653-9053-5cfc8a83dc27"), "600 Nguyễn Văn Cừ", "https://cantho.fpt.edu.vn/Data/Sites/1/media/logo-moi.png", new DateTime(2024, 5, 13, 22, 55, 27, 441, DateTimeKind.Local).AddTicks(2398), "admin@fpt.edu.vn", "Lê Văn Admin", "Nam", false, false, false, false, "Kinh", "0987654321" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "ID", "IsActive", "Password", "RefreshToken", "RefreshTokenExpires", "UserID", "Username" },
                values: new object[,]
                {
                    { "GV0001", true, "$2a$11$HtGmr2ooQQRA2dUleyh6r.krFTYwuIDlqyftJ6C5.2/HkMJ35GXji", "", new DateTime(2024, 5, 13, 22, 55, 27, 612, DateTimeKind.Local).AddTicks(6808), new Guid("f64b2a6a-ee14-4653-9053-5cfc8a83dc27"), "Admin" },
                    { "GV0002", true, "$2a$11$Am3biJaZat4t3aJv.THxfu/39ONZ9i9mqsPlum4Ouw1RqnADlqe92", "", new DateTime(2024, 5, 13, 22, 55, 27, 783, DateTimeKind.Local).AddTicks(4797), new Guid("2cef2fe7-0047-4811-ab43-e608103b3ec3"), "HomeroomTeacher" },
                    { "GV0003", true, "$2a$11$IUfoDLVT7kx2TELn630zkOwiYK3dnzR7V7jgF1mm9EC6JojdXrwe.", "", new DateTime(2024, 5, 13, 22, 55, 27, 954, DateTimeKind.Local).AddTicks(5445), new Guid("3d63767e-e98f-490c-8e34-d7c65fb3aacc"), "SubjectTeacher" },
                    { "GV0004", true, "$2a$11$MfRolm3JxuNY2W77C40CTug6qtLuBo2Iick/5SABQDW/d2k6XLiwe", "", new DateTime(2024, 5, 13, 22, 55, 28, 130, DateTimeKind.Local).AddTicks(8695), new Guid("033f2b95-93aa-46cc-94f2-89a74a865175"), "Supervisor" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionID", "RoleID" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 5 },
                    { 3, 3 },
                    { 3, 4 },
                    { 3, 5 },
                    { 4, 5 },
                    { 5, 2 },
                    { 5, 3 },
                    { 5, 4 },
                    { 5, 5 },
                    { 6, 5 },
                    { 7, 3 },
                    { 7, 4 },
                    { 7, 5 },
                    { 8, 5 },
                    { 9, 2 },
                    { 9, 3 },
                    { 9, 4 },
                    { 9, 5 },
                    { 10, 5 },
                    { 11, 5 },
                    { 12, 5 },
                    { 13, 2 },
                    { 13, 3 },
                    { 13, 4 },
                    { 13, 5 },
                    { 14, 5 },
                    { 15, 5 },
                    { 16, 5 },
                    { 17, 2 },
                    { 17, 3 },
                    { 17, 4 },
                    { 17, 5 },
                    { 18, 5 },
                    { 19, 5 },
                    { 20, 5 },
                    { 21, 2 },
                    { 21, 3 },
                    { 21, 4 },
                    { 21, 5 },
                    { 22, 4 },
                    { 22, 5 },
                    { 23, 3 },
                    { 23, 4 },
                    { 23, 5 },
                    { 24, 4 },
                    { 24, 5 },
                    { 25, 2 },
                    { 25, 3 },
                    { 25, 4 },
                    { 25, 5 },
                    { 26, 4 },
                    { 26, 5 },
                    { 27, 4 },
                    { 27, 5 },
                    { 28, 4 },
                    { 28, 5 },
                    { 29, 2 },
                    { 29, 4 },
                    { 29, 5 },
                    { 30, 4 },
                    { 31, 4 },
                    { 32, 4 },
                    { 33, 2 },
                    { 33, 4 },
                    { 33, 5 },
                    { 34, 5 },
                    { 35, 5 },
                    { 36, 5 },
                    { 37, 2 },
                    { 37, 5 },
                    { 38, 5 },
                    { 39, 5 }
                });

            migrationBuilder.InsertData(
                table: "AccountRoles",
                columns: new[] { "AccountID", "RoleID" },
                values: new object[,]
                {
                    { "GV0001", 1 },
                    { "GV0002", 3 },
                    { "GV0003", 4 },
                    { "GV0004", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountPermissions_AccountID",
                table: "AccountPermissions",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRoles_AccountID",
                table: "AccountRoles",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserID",
                table: "Accounts",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountStudents_RoleID",
                table: "AccountStudents",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStudents_UserID",
                table: "AccountStudents",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_AccountID",
                table: "ActivityLogs",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ScheduleID",
                table: "Attendances",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentID",
                table: "Attendances",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_SchoolYearID",
                table: "Classes",
                column: "SchoolYearID");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TeacherID",
                table: "Classes",
                column: "TeacherID");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentScores_SubjectID",
                table: "ComponentScores",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_LessonsPlans_SubjectID",
                table: "LessonsPlans",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreateBy",
                table: "Notifications",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleID",
                table: "RolePermissions",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ClassID",
                table: "Schedules",
                column: "ClassID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SubjectID",
                table: "Schedules",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_TeacherID",
                table: "Schedules",
                column: "TeacherID");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSettings_CreateBy",
                table: "SchoolSettings",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_ClassID",
                table: "StudentClasses",
                column: "ClassID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentScores_ComponentScoreID",
                table: "StudentScores",
                column: "ComponentScoreID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentScores_SchoolYearID",
                table: "StudentScores",
                column: "SchoolYearID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentScores_StudentID",
                table: "StudentScores",
                column: "StudentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountPermissions");

            migrationBuilder.DropTable(
                name: "AccountRoles");

            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "LessonsPlans");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SchoolSettings");

            migrationBuilder.DropTable(
                name: "StudentClasses");

            migrationBuilder.DropTable(
                name: "StudentScores");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "AccountStudents");

            migrationBuilder.DropTable(
                name: "ComponentScores");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "SchoolYears");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
