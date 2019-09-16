using garda.Models.Data.UserAuthData;
using Microsoft.EntityFrameworkCore;

namespace garda.Models.Context.UserAuthContext
{
    public class UserAuthDbContext : DbContext
    {
        public UserAuthDbContext(DbContextOptions<UserAuthDbContext> options) : base(options)         
        {         
        }       
        public DbSet<UserAuth> UserAuths { get; set; } 
    }
}