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

            modelBuilder.Entity<Classes>()
                .HasOne(c => c.Teacher)
                .WithMany(a => a.Classes)
                .HasForeignKey(c => c.TeacherID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentClasses>()
                .HasOne(sc => sc.Classes)
                .WithMany(c => c.StudentClasses)
                .HasForeignKey(sc => sc.ClassID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Classes)
                .WithMany(c => c.Schedules)
                .HasForeignKey(s => s.ClassID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Subject)
                .WithMany(c => c.Schedules)
                .HasForeignKey(s => s.SubjectID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Permission>()
                .HasData
                (
                    new Permission()
                    {
                        ID = 1,
                        Name = "Admin"
                    },
                    new Permission()
                    {
                        ID = 2,
                        Name = "Add Teacher"
                    },
                    new Permission()
                    {
                        ID = 3,
                        Name = "Update Teacher"
                    },
                    new Permission()
                    {
                        ID = 4,
                        Name = "Delete Teacher"
                    },
                    new Permission()
                    {
                        ID = 5,
                        Name = "Get Teacher"
                    },
                    new Permission()
                    {
                        ID = 6,
                        Name = "Add Student"
                    },
                    new Permission()
                    {
                        ID = 7,
                        Name = "Update Student"
                    },
                    new Permission()
                    {
                        ID = 8,
                        Name = "Delete Student"
                    },
                    new Permission()
                    {
                        ID = 9,
                        Name = "Get Student"
                    },
                    new Permission()
                    {
                        ID = 10,
                        Name = "Add Subject"
                    },
                    new Permission()
                    {
                        ID = 11,
                        Name = "Update Subject"
                    },
                    new Permission()
                    {
                        ID = 12,
                        Name = "Delete Subject"
                    },
                    new Permission()
                    {
                        ID = 13,
                        Name = "Get Subject"
                    },
                    new Permission()
                    {
                        ID = 14,
                        Name = "Add Class"
                    },
                    new Permission()
                    {
                        ID = 15,
                        Name = "Update Class"
                    },
                    new Permission()
                    {
                        ID = 16,
                        Name = "Delete Class"
                    },
                    new Permission()
                    {
                        ID = 17,
                        Name = "Get Class"
                    },
                    new Permission()
                    {
                        ID = 18,
                        Name = "Add Schedule"
                    },
                    new Permission()
                    {
                        ID = 19,
                        Name = "Update Schedule"
                    },
                    new Permission()
                    {
                        ID = 20,
                        Name = "Delete Schedule"
                    },
                    new Permission()
                    {
                        ID = 21,
                        Name = "Get Schedule"
                    },
                    new Permission()
                    {
                        ID = 22,
                        Name = "Add Register Book"
                    },
                    new Permission()
                    {
                        ID = 23,
                        Name = "Update Register Book"
                    },
                    new Permission()
                    {
                        ID = 24,
                        Name = "Delete Register Book"
                    },
                    new Permission()
                    {
                        ID = 25,
                        Name = "Get Register Book"
                    },
                    new Permission()
                    {
                        ID = 26,
                        Name = "Add Attendance"
                    },
                    new Permission()
                    {
                        ID = 27,
                        Name = "Update Attendance"
                    },
                    new Permission()
                    {
                        ID = 28,
                        Name = "Delete Attendance"
                    },
                    new Permission()
                    {
                        ID = 29,
                        Name = "Get Attendance"
                    },
                    new Permission()
                    {
                        ID = 30,
                        Name = "Add Mark"
                    },
                    new Permission()
                    {
                        ID = 31,
                        Name = "Update Mark"
                    },
                    new Permission()
                    {
                        ID = 32,
                        Name = "Delete Mark"
                    },
                    new Permission()
                    {
                        ID = 33,
                        Name = "Get Mark"
                    },
                    new Permission()
                    {
                        ID = 34,
                        Name = "Add Notification"
                    },
                    new Permission()
                    {
                        ID = 35,
                        Name = "Update Notification"
                    },
                    new Permission()
                    {
                        ID = 36,
                        Name = "Delete Notification"
                    },
                    new Permission()
                    {
                        ID = 37,
                        Name = "Get Notification"
                    },
                    new Permission()
                    {
                        ID = 38,
                        Name = "Update Setting"
                    },
                    new Permission()
                    {
                        ID = 39,
                        Name = "Get Log"
                    }
                );

            modelBuilder.Entity<Role>()
                .HasData
                (
                    new Role()
                    {
                        ID = 1,
                        Name = "Admin"
                    },
                    new Role()
                    {
                        ID = 2,
                        Name = "Student"
                    },
                    new Role()
                    {
                        ID = 3,
                        Name = "Homeroom Teacher"
                    },
                    new Role()
                    {
                        ID = 4,
                        Name = "Subject Teacher"
                    },
                    new Role()
                    {
                        ID = 5,
                        Name = "Supervisor"
                    }
                );

            modelBuilder.Entity<RolePermission>()
                .HasData
                (
                    new RolePermission()
                    {
                        RoleID = 1,
                        PermissionID = 1
                    },
                    new RolePermission()
                    {
                        RoleID = 2,
                        PermissionID = 5
                    },
                    new RolePermission()
                    {
                        RoleID = 2,
                        PermissionID = 9
                    },
                    new RolePermission()
                    {
                        RoleID = 2,
                        PermissionID = 13
                    },
                    new RolePermission()
                    {
                        RoleID = 2,
                        PermissionID = 17
                    },
                    new RolePermission()
                    {
                        RoleID = 2,
                        PermissionID = 21
                    },
                    new RolePermission()
                    {
                        RoleID = 2,
                        PermissionID = 25
                    },
                    new RolePermission()
                    {
                        RoleID = 2,
                        PermissionID = 29
                    },
                    new RolePermission()
                    {
                        RoleID = 2,
                        PermissionID = 33
                    },
                    new RolePermission()
                    {
                        RoleID = 2,
                        PermissionID = 37
                    },
                    new RolePermission()
                    {
                        RoleID = 3,
                        PermissionID = 3
                    },
                    new RolePermission()
                    {
                        RoleID = 3,
                        PermissionID = 5
                    },
                    new RolePermission()
                    {
                        RoleID = 3,
                        PermissionID = 7
                    },
                    new RolePermission()
                    {
                        RoleID = 3,
                        PermissionID = 9
                    },
                    new RolePermission()
                    {
                        RoleID = 3,
                        PermissionID = 13
                    },
                    new RolePermission()
                    {
                        RoleID = 3,
                        PermissionID = 17
                    },
                    new RolePermission()
                    {
                        RoleID = 3,
                        PermissionID = 21
                    },
                    new RolePermission()
                    {
                        RoleID = 3,
                        PermissionID = 23
                    },
                    new RolePermission()
                    {
                        RoleID = 3,
                        PermissionID = 25
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 3
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 5
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 7
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 9
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 13
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 17
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 21
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 22
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 23
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 24
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 25
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 26
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 27
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 28
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 29
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 30
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 31
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 32
                    },
                    new RolePermission()
                    {
                        RoleID = 4,
                        PermissionID = 33
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 2
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 3
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 4
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 5
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 6
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 7
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 8
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 9
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 10
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 11
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 12
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 13
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 14
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 15
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 16
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 17
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 18
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 19
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 20
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 21
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 22
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 23
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 24
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 25
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 26
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 27
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 28
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 29
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 33
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 34
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 35
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 36
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 37
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 38
                    },
                    new RolePermission()
                    {
                        RoleID = 5,
                        PermissionID = 39
                    }
                );

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

            userID = Guid.NewGuid();

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
                        ID = "GV0002",
                        IsActive = true,
                        Username = "HomeroomTeacher",
                        Password = BCrypt.Net.BCrypt.HashPassword("aA@123"),
                        RefreshToken = "",
                        RefreshTokenExpires = DateTime.Now,
                        UserID = userID,
                    }
                );

            userID = Guid.NewGuid();

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
                        ID = "GV0003",
                        IsActive = true,
                        Username = "SubjectTeacher",
                        Password = BCrypt.Net.BCrypt.HashPassword("aA@123"),
                        RefreshToken = "",
                        RefreshTokenExpires = DateTime.Now,
                        UserID = userID,
                    }
                );

            userID = Guid.NewGuid();

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
                        ID = "GV0004",
                        IsActive = true,
                        Username = "Supervisor",
                        Password = BCrypt.Net.BCrypt.HashPassword("aA@123"),
                        RefreshToken = "",
                        RefreshTokenExpires = DateTime.Now,
                        UserID = userID,
                    }
                );

            modelBuilder.Entity<AccountRole>()
                .HasData
                (
                    new AccountRole()
                    {
                        AccountID = "GV0001",
                        RoleID = 1,
                    },
                    new AccountRole()
                    {
                        AccountID = "GV0002",
                        RoleID = 3,
                    },
                    new AccountRole()
                    {
                        AccountID = "GV0003",
                        RoleID = 4,
                    },
                    new AccountRole()
                    {
                        AccountID = "GV0004",
                        RoleID = 5,
                    }
                );

            base.OnModelCreating(modelBuilder);
        }

    }
}
