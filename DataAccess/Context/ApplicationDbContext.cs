using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<AccountPermission> AccountPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<AccountRole>()
                .HasKey(ar => new { ar.AccountID, ar.RoleID });

            modelBuilder.Entity<AccountPermission>()
                .HasKey(ag => new { ag.AccountID, ag.PermissionID });

            modelBuilder.Entity<RolePermission>()
                .HasKey(ag => new { ag.RoleID, ag.PermissionID });

        }
        }
}
