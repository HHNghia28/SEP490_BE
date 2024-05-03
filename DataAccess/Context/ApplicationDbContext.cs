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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountPermission>()
                .HasKey(ap => new { ap.PermissionID, ap.AccountID });
            modelBuilder.Entity<AccountRole>()
                .HasKey(ap => new { ap.RoleID, ap.AccountID });
            modelBuilder.Entity<RolePermission>()
                .HasKey(ap => new { ap.PermissionID, ap.RoleID });

            base.OnModelCreating(modelBuilder);
        }

    }
}
