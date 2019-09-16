using garda.Models.Data.UserAuthData;
using Microsoft.EntityFrameworkCore;

namespace garda.Services.Historisation
{
    public class HistoUserAuthDbContext : DbContext
    {
        public HistoUserAuthDbContext(DbContextOptions<HistoUserAuthDbContext> options) : base(options)         
        {         
        }       
        public DbSet<HistoUserAuth> HistoUserAuths { get; set; } 
    }
}