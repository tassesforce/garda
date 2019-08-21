using garda.Models.data.ClientAppData;
using Microsoft.EntityFrameworkCore;

namespace garda.Models.Context.ClientContext
{
    public class ClientAppDbContext : DbContext
    {
        public ClientAppDbContext(DbContextOptions<ClientAppDbContext> options) : base(options)         
        {         
        }       
        public DbSet<ClientApp> ClientApps { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientApp>()
                .HasKey(c => new {c.ClientId, c.ClientSecret});
        }
    }
}