using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Models;
using System;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {

        }
        public FootballBettingContext(DbContextOptions options) 
            : base(options)
        {

        }

        // DbSets
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<User> Users { get; set; }


        // OmConfiguring and OnModelCreating -> No Lazy Loading in EF Core (Lazy Loading is done thrue using Proxies package)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DataCofiguration.ConnectionString);
            }
            //base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(team => 
            {
                //TeamId, Name, LogoUrl, Initials (JUV, LIV, ARS…), Budget, PrimaryKitColorId, SecondaryKitColorId, TownId
                team.HasKey(t => t.TeamId);

                team
                    .Property(t => t.Name)
                    .HasMaxLength(50)
                    .IsRequired(true)
                    .IsUnicode(true);

                team
                    .Property(t => t.LogoUrl)
                    .HasMaxLength(350)
                    .IsUnicode(true)
                    .IsUnicode(false);

                team
                    .Property(t => t.Initials)
                    .HasMaxLength(3)
                    .IsRequired(true)
                    .IsUnicode(true);

                team
                    .Property(t => t.Budget)
                    .IsRequired(true);

                //PrimaryKitColorId
                team
                    .HasOne(t => t.PrimaryKitColor)
                    .WithMany(c => c.PrimaryKitTeams)
                    .HasForeignKey(t => t.PrimaryKitColorId)
                    .OnDelete(DeleteBehavior.Restrict);

                //SecondaryKitColorId
                team
                    .HasOne(t => t.SecondaryKitColor)
                    .WithMany(c => c.SecondaryKitTeams)
                    .HasForeignKey(t => t.SecondaryKitColorId)
                    .OnDelete(DeleteBehavior.Restrict);

                //TownId
                team
                    .HasOne(t => t.Town)
                    .WithMany(town => town.Teams)
                    .HasForeignKey(t => t.TownId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Color>(color =>
            {
                //ColorId, Name
                color.HasKey(c => c.ColorId);

                color
                    .Property(c => c.Name)
                    .HasMaxLength(50)
                    .IsRequired(true)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Town>(town =>
            {
                //TownId, Name, CountryId
                town.HasKey(t => t.TownId);

                town
                    .Property(t => t.Name)
                    .HasMaxLength(50)
                    .IsRequired(true)
                    .IsUnicode(true);

                //CountryId
                town
                    .HasOne(t => t.Country)
                    .WithMany(c => c.Towns)
                    .HasForeignKey(t => t.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Country>(country =>
            {
                //CountryId, Name
                country.HasKey(c => c.CountryId);

                country
                    .Property(c => c.Name)
                    .HasMaxLength(50)
                    .IsRequired(true)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<Player>(player =>
            {
                //PlayerId, Name, SquadNumber, TeamId, PositionId, IsInjured
                player.HasKey(p => p.PlayerId);

                player
                    .Property(p => p.Name)
                    .HasMaxLength(80)
                    .IsRequired(true)
                    .IsUnicode(true);

                player
                    .Property(p => p.SquadNumber)
                    .HasMaxLength(3)
                    .IsUnicode(false);

                //TeamId
                player
                    .HasOne(p => p.Team)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                //PositionId
                player
                    .HasOne(p => p.Position)
                    .WithMany(pos => pos.Players)
                    .HasForeignKey(p => p.PositionId)
                    .OnDelete(DeleteBehavior.Restrict);

                player
                    .Property(p => p.IsInjured)
                    .IsRequired(true);
            });

            modelBuilder.Entity<Position>(position =>
            {
                //PositionId, Name
                position.HasKey(p => p.PositionId);

                position
                    .Property(p => p.Name)
                    .HasMaxLength(30)
                    .IsRequired(true)
                    .IsUnicode(true);
            });

            modelBuilder.Entity<PlayerStatistic>(playerStat =>
            {
                //GameId, PlayerId, ScoredGoals, Assists, MinutesPlayed
                playerStat.HasKey(ps => new { ps.GameId, ps.PlayerId });

                //GameId
                playerStat
                    .HasOne(ps => ps.Game)
                    .WithMany(g => g.PlayerStatistics)
                    .HasForeignKey(ps => ps.GameId)
                    .OnDelete(DeleteBehavior.Restrict);

                //PlayerId
                playerStat
                    .HasOne(ps => ps.Player)
                    .WithMany(p => p.PlayerStatistics)
                    .HasForeignKey(ps => ps.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);

                //ScoredGoals    <-> can use 'byte'
                playerStat
                    .Property(ps => ps.ScoredGoals)
                    .IsRequired(true);

                //Assists  <-> can use 'byte'
                playerStat
                    .Property(ps => ps.Assists)
                    .IsRequired(true);

                //MinutesPlayed
                playerStat
                    .Property(ps => ps.MinutesPlayed)
                    .IsRequired(true);
            });

            modelBuilder.Entity<Game>(game =>
            {
                //GameId, HomeTeamId, AwayTeamId, 
                //HomeTeamGoals, AwayTeamGoals, DateTime, 
                //HomeTeamBetRate, AwayTeamBetRate, DrawBetRate, Result

                game.HasKey(g => g.GameId);

                //DateTime
                game
                    .Property(g => g.DateTime)
                    .IsRequired(true);

                //Result
                game
                    .Property(g => g.Result)
                    .HasMaxLength(10)
                    .IsRequired(false)
                    .IsUnicode(false);

                //HomeTeamId
                game
                    .HasOne(g => g.HomeTeam)
                    .WithMany(ht => ht.HomeGames)
                    .HasForeignKey(g => g.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                //AwayTeamId
                game
                    .HasOne(g => g.AwayTeam)
                    .WithMany(at => at.AwayGames)
                    .HasForeignKey(g => g.AwayTeamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bet>(bet =>
            {
                //BetId, Amount, Prediction, DateTime, UserId, GameId
                bet.HasKey(b => b.BetId);

                bet
                    .Property(b => b.Amount)
                    .IsRequired(true);

                //bet
                //    .Property(b => b.Prediction);   <-> DON HAVE Access to it!

                //UserId
                bet
                    .HasOne(b => b.User)
                    .WithMany(u => u.Bets)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                //GameId
                bet
                    .HasOne(b => b.Game)
                    .WithMany(g => g.Bets)
                    .HasForeignKey(b => b.GameId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(user =>
            {
                //UserId, Username, Password, Email, Name, Balance
                user.HasKey(u => u.UserId);

                user
                    .Property(u => u.Username)
                    .HasMaxLength(30)
                    .IsRequired(true)
                    .IsUnicode(false);

                user
                    .Property(u => u.Password)
                    .HasMaxLength(256)
                    .IsRequired(true)
                    .IsUnicode(false);

                user
                    .Property(u => u.Email)
                    .HasMaxLength(50)
                    .IsRequired(true)
                    .IsUnicode(false);

                user
                    .Property(u => u.Name)
                    .HasMaxLength(80)
                    .IsRequired(true)
                    .IsUnicode(true);

            });
        }
    }
}
