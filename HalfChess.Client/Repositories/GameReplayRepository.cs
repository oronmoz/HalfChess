using HalfChess.Client.Data;
using HalfChess.Client.Data.Models;
using HalfChess.Core.Domain;
using HalfChess.Core.Domain.Pieces;
using HalfChess.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HalfChess.Client.Repositories
{
    public class GameReplayRepository : IGameReplayRepository
    {
        private readonly GameReplayContext _context;
        private readonly ILogger<GameReplayRepository> _logger;

        public GameReplayRepository(GameReplayContext context, ILogger<GameReplayRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveMove(Guid gameId, Move move)
        {
            try
            {
                var clientMove = new ClientMove
                {
                    GameId = gameId,
                    FromPosition = move.From.ToAlgebraic(),
                    ToPosition = move.To.ToAlgebraic(),
                    PieceType = move.Piece.Type.ToString(), 
                    IsCapture = move.IsCapture,
                    MoveTime = DateTime.UtcNow
                };

                _context.Moves.Add(clientMove);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving move for game {GameId}", gameId);
                throw;
            }
        }

        public async Task<IEnumerable<Move>> GetMoves(Guid gameId)
        {
            try
            {
                var moves = await _context.Moves
                    .Where(m => m.GameId == gameId)
                    .OrderBy(m => m.MoveTime)
                    .ToListAsync();

                return moves.Select(m =>
                {
                    var from = Position.FromAlgebraic(m.FromPosition);
                    var to = Position.FromAlgebraic(m.ToPosition);
                    var pieceType = Enum.Parse<PieceType>(m.PieceType); // Fixed: Parse the stored piece type string

                    // Create a temporary piece for the move record
                    var piece = CreatePiece(pieceType, PieceColor.White, from); // Color doesn't matter for replay

                    return new Move(from, to, piece, m.IsCapture);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting moves for game {GameId}", gameId);
                throw;
            }
        }

        public async Task<IEnumerable<GameReplaySummary>> GetGameReplays()
        {
            try
            {
                return await _context.Games
                    .Select(g => new GameReplaySummary
                    {
                        GameId = g.GameId,
                        GameDate = g.GameDate,
                        Result = g.Result,
                        TotalMoves = g.Moves.Count
                    })
                    .OrderByDescending(g => g.GameDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game replays");
                throw;
            }
        }

        private static Piece CreatePiece(PieceType type, PieceColor color, Position position)
        {
            return type switch
            {
                PieceType.King => new King(color, position),
                PieceType.Bishop => new Bishop(color, position),
                PieceType.Knight => new Knight(color, position),
                PieceType.Rook => new Rook(color, position),
                PieceType.Pawn => new Pawn(color, position),
                _ => throw new ArgumentException($"Unknown piece type: {type}")
            };
        }
    }
}