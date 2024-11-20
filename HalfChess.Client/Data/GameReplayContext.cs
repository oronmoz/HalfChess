using Microsoft.EntityFrameworkCore;
using HalfChess.Client.Data.Models;

namespace HalfChess.Client.Data
{
    public class GameReplayContext : DbContext
    {
        public DbSet<ClientGame> Games { get; set; } = null!;
        public DbSet<ClientMove> Moves { get; set; } = null!;

        public GameReplayContext(DbContextOptions<GameReplayContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientGame>(entity =>
            {
                entity.HasKey(e => e.GameId);
                entity.HasMany(e => e.Moves)
                      .WithOne(e => e.Game)
                      .HasForeignKey(e => e.GameId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ClientMove>(entity =>
            {
                entity.HasKey(e => e.MoveId);
            });
        }
    }
}