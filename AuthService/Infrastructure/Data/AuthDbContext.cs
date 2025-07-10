using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserRole composite key (UserId, RoleId, ScopeDepartmentId)
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId, ur.ScopeDepartmentId });

            // RolePermission composite key (RoleId, PermissionId)
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // UserLoginHistory PK
            modelBuilder.Entity<UserLoginHistory>()
                .HasKey(ulh => ulh.Id);

            // Department PK
            modelBuilder.Entity<Department>()
                .HasKey(d => d.Id);

            // Permission PK
            modelBuilder.Entity<Permission>()
                .HasKey(p => p.Id);

            // Role PK
            modelBuilder.Entity<Role>()
                .HasKey(r => r.Id);

            // User PK
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            // User-Department relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany()
                .HasForeignKey(u => u.DepartmentId);

            // UserRole relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.ScopeDepartment)
                .WithMany()
                .HasForeignKey(ur => ur.ScopeDepartmentId);

            // RolePermission relationships
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleId);
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId);

            // UserLoginHistory relationship
            modelBuilder.Entity<UserLoginHistory>()
                .HasOne(ulh => ulh.User)
                .WithMany()
                .HasForeignKey(ulh => ulh.UserId);

            // RefreshToken PK
            modelBuilder.Entity<RefreshToken>()
                .HasKey(rt => rt.Id);

            // RefreshToken relationship
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId);
        }
    }
}           