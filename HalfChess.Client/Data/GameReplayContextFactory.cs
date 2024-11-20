using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HalfChess.Client.Data
{
    public class GameReplayContextFactory : IDesignTimeDbContextFactory<GameReplayContext>
    {
        public GameReplayContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GameReplayContext>();
            optionsBuilder.UseSqlite("Data Source=GameReplay.db");

            return new GameReplayContext(optionsBuilder.Options);
        }
    }
}
