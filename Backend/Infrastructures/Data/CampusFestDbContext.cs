using Backend.Cores.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructures.Data
{
    public class CampusFestDbContext: DbContext
    {
        public CampusFestDbContext(DbContextOptions options): base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }
}
