using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using HalfChess.Data.Context;

namespace HalfChess.Data
{
    public class HalfChessDbContextFactory : IDesignTimeDbContextFactory<HalfChessDbContext>
    {
        public HalfChessDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HalfChessDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=HalfChessDataDB;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new HalfChessDbContext(optionsBuilder.Options);
        }
    }
}