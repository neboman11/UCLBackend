using Microsoft.EntityFrameworkCore;

namespace UCLBackend.Service.DataAccess.Models
{
    public class UCLContext : DbContext
    {
        public UCLContext(DbContextOptions<UCLContext> options) : base(options)
        {

        }
        
        public DbSet<Player> Players { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Team> Roster { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Standing> Standings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().Property(p => p.IsFreeAgent).HasDefaultValue(true);

            modelBuilder.Entity<Account>(e =>
            {
                e.HasIndex(e => new { e.AccountName, e.Platform }).IsUnique();
            });
        }
    }
}
