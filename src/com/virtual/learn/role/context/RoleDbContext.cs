using garda.Models.Data.RoleData;
using garda.Models.Data.UserAuthData;
using garda.Models.Data.UserRoleData;
using Microsoft.EntityFrameworkCore;

namespace garda.Models.Context.RoleContext
{
    public class RoleDbContext : DbContext
    {
        public RoleDbContext(DbContextOptions<RoleDbContext> options) : base(options)         
        {         
        }       
        public DbSet<Role> Roles { get; set; } 
        public DbSet<UserRole> UserRoles { get; set; } 


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(c => new {c.UserId, c.IdRole});
        }
    }
}