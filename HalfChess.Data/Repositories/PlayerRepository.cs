using HalfChess.Core.Domain.Models;
using HalfChess.Core.Interfaces;
using HalfChess.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HalfChess.Data.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly HalfChessDbContext _context;
        private readonly ILogger<PlayerRepository> _logger;

        public PlayerRepository(HalfChessDbContext context, ILogger<PlayerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsPlayerIdTaken(int playerId)
        {
            return await _context.Players
                .AnyAsync(p => p.PlayerId == playerId);
        }

        public async Task AddPlayer(Player player)
        {
            if (player.PlayerId < 1 || player.PlayerId > 1000)
            {
                throw new ArgumentException("Player ID must be between 1 and 1000");
            }

            if (await IsPlayerIdTaken(player.PlayerId))
            {
                throw new InvalidOperationException($"Player ID {player.PlayerId} is already taken");
            }

            if (player.FirstName.Length < 2)
            {
                throw new ArgumentException("First name must be at least 2 characters long");
            }

            try
            {
                _context.Players.Add(player);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Player {PlayerId} registered successfully", player.PlayerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding player {PlayerId}", player.PlayerId);
                throw;
            }
        
    }

        public async Task<Player> GetPlayer(int playerId)
        {
            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
            {
                throw new KeyNotFoundException($"Player {playerId} not found. Please register first.");
            }

            return player;
        }
    }
}