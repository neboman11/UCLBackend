using Microsoft.EntityFrameworkCore;

namespace UCLBackend.DataAccess.Models
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
            modelBuilder.Entity<Player>().ToTable("Players");
            modelBuilder.Entity<Account>().ToTable("Accounts");
            modelBuilder.Entity<Team>().ToTable("Roster");
        }
    }
}
