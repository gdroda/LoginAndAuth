using LoginAndAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginAndAuth.Data
{
    public class LoginContext: DbContext
    {
        public LoginContext(DbContextOptions options): base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
