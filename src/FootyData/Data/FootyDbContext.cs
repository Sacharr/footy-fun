using Microsoft.EntityFrameworkCore;
using FootyData.Models;

namespace FootyData.Data
{
    public class FootyDbContext : DbContext
    {
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }

        public FootyDbContext(DbContextOptions<FootyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Competition configuration
            modelBuilder.Entity<Competition>()
                .HasKey(c => c.Id);

            // Team configuration
            modelBuilder.Entity<Team>()
                .HasKey(t => t.Id);

            // Match configuration
            modelBuilder.Entity<Match>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Match>()
                .Property(m => m.UtcDate)
                .HasColumnType("TEXT"); // SQLite stores datetime as TEXT
        }

        public static void EnsureCreated(string connectionString)
        {
            var options = new DbContextOptionsBuilder<FootyDbContext>()
                .UseSqlite(connectionString)
                .Options;

            using var context = new FootyDbContext(options);
            context.Database.EnsureCreated();
        }
    }

    // Database entity models (separate from API models to keep concerns separate)
    public class Competition
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class Team
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CompetitionId { get; set; }
    }

    public class Match
    {
        public int Id { get; set; }
        public DateTime UtcDate { get; set; }
        public string? Status { get; set; }
        public int? HomeTeamId { get; set; }
        public int? AwayTeamId { get; set; }
        public Team? HomeTeam { get; set; }
        public Team? AwayTeam { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public int CompetitionId { get; set; }
    }
}
