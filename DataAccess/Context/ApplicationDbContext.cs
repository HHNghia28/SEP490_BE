using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<AccountPermission> AccountPermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AccountStudent> AccountStudents { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Classes> Classes { get; set; }
        public DbSet<ComponentScore> ComponentScores { get; set; }
        public DbSet<LessonPlans> LessonsPlans { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<SchoolSetting> SchoolSettings { get; set; }
        public DbSet<SchoolYear> SchoolYears { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentClasses> StudentClasses { get; set; }
        public DbSet<StudentScores> StudentScores { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountPermission>()
                .HasKey(ap => new { ap.PermissionID, ap.AccountID });
            modelBuilder.Entity<AccountRole>()
                .HasKey(ap => new { ap.RoleID, ap.AccountID });
            modelBuilder.Entity<RolePermission>()
                .HasKey(ap => new { ap.PermissionID, ap.RoleID });
            modelBuilder.Entity<StudentClasses>()
                .HasKey(ap => new { ap.StudentID, ap.ClassID });

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Classes) // chỉ định quan hệ một-đến-nhiều
                .WithMany(c => c.Schedules) // Classes có nhiều Schedules
                .HasForeignKey(s => s.ClassID) // khóa ngoại là ClassID
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Subject) // chỉ định quan hệ một-đến-nhiều
                .WithMany(c => c.Schedules) // Classes có nhiều Schedules
                .HasForeignKey(s => s.SubjectID) // khóa ngoại là ClassID
                .OnDelete(DeleteBehavior.NoAction);

            Guid userID = Guid.NewGuid();

            modelBuilder.Entity<User>()
                .HasData
                (
                    new User()
                    {
                        ID = userID,
                        Address = "600 Nguyễn Văn Cừ",
                        Avatar = "https://cantho.fpt.edu.vn/Data/Sites/1/media/logo-moi.png",
                        Birthday = DateTime.Now,
                        Email = "admin@fpt.edu.vn",
                        Fullname = "Lê Văn Admin",
                        Gender = "Nam",
                        Nation = "Kinh",
                        Phone = "0987654321",
                        IsBachelor = false,
                        IsDoctor = false,
                        IsMaster = false,
                        IsProfessor = false,
                    }
                );

            modelBuilder.Entity<Account>()
                .HasData
                (
                    new Account()
                    {
                        ID = "GV0001",
                        IsActive = true,
                        Username = "Admin",
                        Password = BCrypt.Net.BCrypt.HashPassword("aA@123"),
                        RefreshToken = "",
                        RefreshTokenExpires = DateTime.Now,
                        UserID = userID,
                    }
                );

            base.OnModelCreating(modelBuilder);
        }

    }
}
