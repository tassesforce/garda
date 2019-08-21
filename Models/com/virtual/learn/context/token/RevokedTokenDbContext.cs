using garda.Models.Data.RevokedToken;
using garda.Models.Data.UserAuthData;
using Microsoft.EntityFrameworkCore;

namespace garda.Models.Context.TokenContext
{
    public class RevokedTokenDbContext : DbContext
    {
        public RevokedTokenDbContext(DbContextOptions<RevokedTokenDbContext> options) : base(options)         
        {         
        }       
        public DbSet<RevokedToken> RevokedTokens { get; set; } 
    }
}