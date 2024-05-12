﻿// <auto-generated />
using System;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace SEP490_API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BusinessObject.Entities.Account", b =>
                {
                    b.Property<string>("ID")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("RefreshTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("Accounts");

                    b.HasData(
                        new
                        {
                            ID = "GV0001",
                            IsActive = true,
                            Password = "$2a$11$usZL/1Q23wBX830aRLn2sOO3tLJ6xKaXREGAMJYtSuN.tEPjQsgEu",
                            RefreshToken = "",
                            RefreshTokenExpires = new DateTime(2024, 5, 12, 12, 34, 29, 765, DateTimeKind.Local).AddTicks(7491),
                            UserID = new Guid("80c09260-c66b-4e12-a24c-bf72e33bf95b"),
                            Username = "Admin"
                        });
                });

            modelBuilder.Entity("BusinessObject.Entities.AccountPermission", b =>
                {
                    b.Property<Guid>("PermissionID")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(0);

                    b.Property<string>("AccountID")
                        .HasColumnType("nvarchar(50)")
                        .HasColumnOrder(1);

                    b.HasKey("PermissionID", "AccountID");

                    b.HasIndex("AccountID");

                    b.ToTable("AccountPermissions");
                });

            modelBuilder.Entity("BusinessObject.Entities.AccountRole", b =>
                {
                    b.Property<Guid>("RoleID")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(0);

                    b.Property<string>("AccountID")
                        .HasColumnType("nvarchar(50)")
                        .HasColumnOrder(1);

                    b.HasKey("RoleID", "AccountID");

                    b.HasIndex("AccountID");

                    b.ToTable("AccountRoles");
                });

            modelBuilder.Entity("BusinessObject.Entities.AccountStudent", b =>
                {
                    b.Property<string>("ID")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("RefreshTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RoleID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("RoleID");

                    b.HasIndex("UserID");

                    b.ToTable("AccountStudents");
                });

            modelBuilder.Entity("BusinessObject.Entities.ActivityLog", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountID")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("AccountID");

                    b.ToTable("ActivityLogs");
                });

            modelBuilder.Entity("BusinessObject.Entities.Attendance", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Present")
                        .HasColumnType("bit");

                    b.Property<Guid>("ScheduleID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("StudentID")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("ScheduleID");

                    b.HasIndex("StudentID");

                    b.ToTable("Attendances");
                });

            modelBuilder.Entity("BusinessObject.Entities.Classes", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Classroom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SchoolYearID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TeacherID")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("SchoolYearID");

                    b.HasIndex("TeacherID");

                    b.ToTable("Classes");
                });

            modelBuilder.Entity("BusinessObject.Entities.ComponentScore", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<decimal>("ScoreFactor")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Semester")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<Guid>("SubjectID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ID");

                    b.HasIndex("SubjectID");

                    b.ToTable("ComponentScores");
                });

            modelBuilder.Entity("BusinessObject.Entities.LessonPlans", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Slot")
                        .HasColumnType("int");

                    b.Property<Guid>("SubjectID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("ID");

                    b.HasIndex("SubjectID");

                    b.ToTable("LessonsPlans");
                });

            modelBuilder.Entity("BusinessObject.Entities.Notification", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreateBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Thumbnail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("CreateBy");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("BusinessObject.Entities.Permission", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("BusinessObject.Entities.Role", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("BusinessObject.Entities.RolePermission", b =>
                {
                    b.Property<Guid>("PermissionID")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(0);

                    b.Property<Guid>("RoleID")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(1);

                    b.HasKey("PermissionID", "RoleID");

                    b.HasIndex("RoleID");

                    b.ToTable("RolePermissions");
                });

            modelBuilder.Entity("BusinessObject.Entities.Schedule", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClassID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Rank")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("SlotByDate")
                        .HasColumnType("int");

                    b.Property<int>("SlotByLessonPlans")
                        .HasColumnType("int");

                    b.Property<Guid>("SubjectID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TeacherID")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("ClassID");

                    b.HasIndex("SubjectID");

                    b.HasIndex("TeacherID");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("BusinessObject.Entities.SchoolSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreateBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("SchoolAddress")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("SchoolEmail")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SchoolLevel")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("SchoolName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SchoolPhone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.ToTable("SchoolSettings");
                });

            modelBuilder.Entity("BusinessObject.Entities.SchoolYear", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("SchoolYears");
                });

            modelBuilder.Entity("BusinessObject.Entities.Student", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("date");

                    b.Property<string>("Birthplace")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FatherFullName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FatherPhone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("FatherProfession")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("HomeTown")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<bool?>("IsMartyrs")
                        .HasColumnType("bit");

                    b.Property<string>("MotherFullName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MotherPhone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("MotherProfession")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Nation")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("ID");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("BusinessObject.Entities.StudentClasses", b =>
                {
                    b.Property<string>("StudentID")
                        .HasColumnType("nvarchar(50)")
                        .HasColumnOrder(1);

                    b.Property<Guid>("ClassID")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(0);

                    b.HasKey("StudentID", "ClassID");

                    b.HasIndex("ClassID");

                    b.ToTable("StudentClasses");
                });

            modelBuilder.Entity("BusinessObject.Entities.StudentScores", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ComponentScoreID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SchoolYearID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Score")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("StudentID")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("ComponentScoreID");

                    b.HasIndex("SchoolYearID");

                    b.HasIndex("StudentID");

                    b.ToTable("StudentScores");
                });

            modelBuilder.Entity("BusinessObject.Entities.Subject", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Grade")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("BusinessObject.Entities.User", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<bool?>("IsBachelor")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsDoctor")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsMaster")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsProfessor")
                        .HasColumnType("bit");

                    b.Property<string>("Nation")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("ID");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            ID = new Guid("80c09260-c66b-4e12-a24c-bf72e33bf95b"),
                            Address = "600 Nguyễn Văn Cừ",
                            Avatar = "https://cantho.fpt.edu.vn/Data/Sites/1/media/logo-moi.png",
                            Birthday = new DateTime(2024, 5, 12, 12, 34, 29, 578, DateTimeKind.Local).AddTicks(5462),
                            Email = "admin@fpt.edu.vn",
                            Fullname = "Lê Văn Admin",
                            Gender = "Nam",
                            IsBachelor = false,
                            IsDoctor = false,
                            IsMaster = false,
                            IsProfessor = false,
                            Nation = "Kinh",
                            Phone = "0987654321"
                        });
                });

            modelBuilder.Entity("BusinessObject.Entities.Account", b =>
                {
                    b.HasOne("BusinessObject.Entities.User", "User")
                        .WithOne("Account")
                        .HasForeignKey("BusinessObject.Entities.Account", "UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Entities.AccountPermission", b =>
                {
                    b.HasOne("BusinessObject.Entities.Account", "Account")
                        .WithMany("AccountPermissions")
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.Permission", "Permission")
                        .WithMany("AccountPermissions")
                        .HasForeignKey("PermissionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Permission");
                });

            modelBuilder.Entity("BusinessObject.Entities.AccountRole", b =>
                {
                    b.HasOne("BusinessObject.Entities.Account", "Account")
                        .WithMany("AccountRoles")
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.Role", "Role")
                        .WithMany("AccountRoles")
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BusinessObject.Entities.AccountStudent", b =>
                {
                    b.HasOne("BusinessObject.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.Student", "Student")
                        .WithMany("AccountStudents")
                        .HasForeignKey("UserID");

                    b.Navigation("Role");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("BusinessObject.Entities.ActivityLog", b =>
                {
                    b.HasOne("BusinessObject.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObject.Entities.Attendance", b =>
                {
                    b.HasOne("BusinessObject.Entities.Schedule", "Schedule")
                        .WithMany()
                        .HasForeignKey("ScheduleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.AccountStudent", "AccountStudent")
                        .WithMany()
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountStudent");

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("BusinessObject.Entities.Classes", b =>
                {
                    b.HasOne("BusinessObject.Entities.SchoolYear", "SchoolYear")
                        .WithMany()
                        .HasForeignKey("SchoolYearID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.Account", "Teacher")
                        .WithMany()
                        .HasForeignKey("TeacherID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SchoolYear");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("BusinessObject.Entities.ComponentScore", b =>
                {
                    b.HasOne("BusinessObject.Entities.Subject", "Subject")
                        .WithMany("ComponentScores")
                        .HasForeignKey("SubjectID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("BusinessObject.Entities.LessonPlans", b =>
                {
                    b.HasOne("BusinessObject.Entities.Subject", "Subject")
                        .WithMany("LessonPlans")
                        .HasForeignKey("SubjectID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("BusinessObject.Entities.Notification", b =>
                {
                    b.HasOne("BusinessObject.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObject.Entities.RolePermission", b =>
                {
                    b.HasOne("BusinessObject.Entities.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BusinessObject.Entities.Schedule", b =>
                {
                    b.HasOne("BusinessObject.Entities.Classes", "Classes")
                        .WithMany("Schedules")
                        .HasForeignKey("ClassID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.Subject", "Subject")
                        .WithMany("Schedules")
                        .HasForeignKey("SubjectID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.Account", "Teacher")
                        .WithMany()
                        .HasForeignKey("TeacherID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Classes");

                    b.Navigation("Subject");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("BusinessObject.Entities.SchoolSetting", b =>
                {
                    b.HasOne("BusinessObject.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusinessObject.Entities.StudentClasses", b =>
                {
                    b.HasOne("BusinessObject.Entities.Classes", "Classes")
                        .WithMany("StudentClasses")
                        .HasForeignKey("ClassID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.AccountStudent", "AccountStudent")
                        .WithMany()
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountStudent");

                    b.Navigation("Classes");
                });

            modelBuilder.Entity("BusinessObject.Entities.StudentScores", b =>
                {
                    b.HasOne("BusinessObject.Entities.ComponentScore", "ComponentScore")
                        .WithMany()
                        .HasForeignKey("ComponentScoreID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.SchoolYear", "SchoolYear")
                        .WithMany()
                        .HasForeignKey("SchoolYearID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.AccountStudent", "AccountStudent")
                        .WithMany()
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountStudent");

                    b.Navigation("ComponentScore");

                    b.Navigation("SchoolYear");
                });

            modelBuilder.Entity("BusinessObject.Entities.Account", b =>
                {
                    b.Navigation("AccountPermissions");

                    b.Navigation("AccountRoles");
                });

            modelBuilder.Entity("BusinessObject.Entities.Classes", b =>
                {
                    b.Navigation("Schedules");

                    b.Navigation("StudentClasses");
                });

            modelBuilder.Entity("BusinessObject.Entities.Permission", b =>
                {
                    b.Navigation("AccountPermissions");

                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("BusinessObject.Entities.Role", b =>
                {
                    b.Navigation("AccountRoles");

                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("BusinessObject.Entities.Student", b =>
                {
                    b.Navigation("AccountStudents");
                });

            modelBuilder.Entity("BusinessObject.Entities.Subject", b =>
                {
                    b.Navigation("ComponentScores");

                    b.Navigation("LessonPlans");

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("BusinessObject.Entities.User", b =>
                {
                    b.Navigation("Account")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
