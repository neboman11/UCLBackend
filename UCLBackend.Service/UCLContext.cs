using Microsoft.EntityFrameworkCore;
using UCLBackend.DataAccess.Models;

namespace UCLBackend.Service
{
    public class UCLContext : DbContext
    {
        public UCLContext(DbContextOptions<UCLContext> options) : base(options)
        {

        }
        
        public DbSet<Player> Players { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().ToTable("Player");
            modelBuilder.Entity<Account>().ToTable("Account");
        }
    }
}
