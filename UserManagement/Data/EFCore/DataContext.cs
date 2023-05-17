using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.EFCore
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<RoleAssignment> RoleAssignments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PermissionAssignment> PermissionAssignments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
    }
}
