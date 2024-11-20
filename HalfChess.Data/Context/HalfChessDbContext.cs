// File: HalfChess.Data/Context/HalfChessDbContext.cs
using HalfChess.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HalfChess.Data.Context
{
    public class HalfChessDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<GameMove> GameMoves { get; set; } = null!;

        public HalfChessDbContext(DbContextOptions<HalfChessDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.PlayerId);
                entity.Property(e => e.PlayerId)
            .ValueGeneratedNever()
            .HasAnnotation("SqlServer:Identity", null);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.GameId);
                entity.HasOne(e => e.Player)
                      .WithMany(p => p.Games)
                      .HasForeignKey(e => e.PlayerId);
            });

            modelBuilder.Entity<GameMove>(entity =>
            {
                entity.HasKey(e => e.MoveId);
                entity.HasOne(e => e.Game)
                      .WithMany(g => g.Moves)
                      .HasForeignKey(e => e.GameId);
            });
        }
    }
}
