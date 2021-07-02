using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UCLBackend.Service
{
    public class UCLContext : DbContext
    {
        public UCLContext(DbContextOptions<UCLContext> options) : base(options)
        {

        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<User>(entity => {
            //     entity.Property(m => m.DiscordID).HasMaxLength(127).IsRequired();
            //     entity.Property(m => m.Name).HasMaxLength(127).IsRequired();
            //     entity.Property(m => m.UCLID).HasMaxLength(127).IsRequired();
            //     entity.Property(m => m.System).HasMaxLength(127).IsRequired();
            //     entity.Property(m => m.TimeZone).HasMaxLength(127);
            //     entity.Property(m => m.Team).HasMaxLength(127);
            //     entity.Property(m => m.League).HasMaxLength(127);
            //     entity.Property(m => m.PeakMMR).HasDefaultValue(0);
            //     entity.Property(m => m.Salary).HasPrecision(1).HasDefaultValue(0.0);
            // });
            // modelBuilder.Entity<Account>(entity => {
            //     entity.Property(m => m.AccountID).HasMaxLength(127).IsRequired();
            //     entity.Property(m => m.Platform).HasMaxLength(127).IsRequired();
            // }).ToTable("Accounts");
            modelBuilder.Entity<User>().HasKey(m => new { m.UCLID });
            modelBuilder.Entity<Account>().HasKey(m => new { m.UCLID });
        }
    }

    public class User
    {
        public string DiscordID { get; set; }
        public string Name { get; set; }
        public string UCLID { get; set; }
        public string System { get; set; }
        public string TimeZone { get; set; }
        public string Team { get; set; }
        public string League { get; set; }
        public int PeakMMR { get; set; }
        public double Salary { get; set; }
    }

    public class Account
    {
        public string AccountID { get; set; }
        public string Platform { get; set; }
        public string UCLID { get; set; }
    }
}
