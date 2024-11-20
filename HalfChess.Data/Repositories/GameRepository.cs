using HalfChess.Core.Domain.Models;
using HalfChess.Core.Interfaces;
using HalfChess.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HalfChess.Data.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly HalfChessDbContext _context;
        private readonly ILogger<GameRepository> _logger;

        public GameRepository(HalfChessDbContext context, ILogger<GameRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Game> CreateGame(int playerId, int timePerMove)
        {
            var game = new Game
            {
                PlayerId = playerId,
                StartTime = DateTime.UtcNow,
                TimePerMove = timePerMove
            };

            try
            {
                _context.Games.Add(game);
                await _context.SaveChangesAsync();
                return game;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating game for player {PlayerId}", playerId);
                throw;
            }
        }

        public async Task<Game> GetGame(int gameId)
        {
            return await _context.Games
                .Include(g => g.Player)
                .Include(g => g.Moves)
                .FirstOrDefaultAsync(g => g.GameId == gameId)
                ?? throw new KeyNotFoundException($"Game {gameId} not found");
        }

        public async Task<IEnumerable<Game>> GetPlayerGames(int playerId)
        {
            return await _context.Games
                .Include(g => g.Moves)
                .Where(g => g.PlayerId == playerId)
                .OrderByDescending(g => g.StartTime)
                .ToListAsync();
        }

        public async Task RecordMove(string gameId, string fromPosition, string toPosition, bool isPlayerMove)
        {
            if (!int.TryParse(gameId, out int id))
                throw new ArgumentException("Invalid game ID", nameof(gameId));

            var move = new GameMove
            {
                GameId = id,
                FromPosition = fromPosition,
                ToPosition = toPosition,
                MoveTime = DateTime.UtcNow,
                IsPlayerMove = isPlayerMove
            };

            try
            {
                _context.GameMoves.Add(move);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording move for game {GameId}", gameId);
                throw;
            }
        }

        public async Task EndGame(string gameId, string result)
        {
            if (!int.TryParse(gameId, out int id))
                throw new ArgumentException("Invalid game ID", nameof(gameId));

            var game = await _context.Games.FindAsync(id);
            if (game == null)
                throw new KeyNotFoundException($"Game {gameId} not found");

            game.EndTime = DateTime.UtcNow;
            game.Result = result;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending game {GameId}", gameId);
                throw;
            }
        }
    }
}