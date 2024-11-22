using HalfChess.Core.Domain;
using HalfChess.Core.Domain.Models;
using HalfChess.Core.Game;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace HalfChess.Client.Services
{
    public interface IGameService
    {
        Task<GameStartResponse> StartGame(GameStartRequest request);
        Task<MoveResponse> MakeMove(Guid gameId, MoveRequest request);
        Task<GameStateResponse> GetGameState(Guid gameId);
        Task ResignGame(Guid gameId);
    }

    public class GameStartRequest
    {
        public int PlayerId { get; set; }
        public int TimePerMove { get; set; } = 60;
    }

    public class GameStartResponse
    {
        public Guid GameId { get; set; }
        public IReadOnlyBoard InitialBoard { get; set; } = null!;
        public PieceColor PlayerColor { get; set; }
        public int TimePerMove { get; set; }
    }

    public class MoveRequest
    {
        public string FromPosition { get; set; } = string.Empty;
        public string ToPosition { get; set; } = string.Empty;
    }

    public class MoveResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public GameState GameState { get; set; }
        public IReadOnlyBoard BoardState { get; set; } = null!;
        public PieceColor CurrentTurn { get; set; }
        public DateTime LastMoveTime { get; set; }
    }

    public class GameStateResponse
    {
        public GameState State { get; set; }
        public PieceColor CurrentTurn { get; set; }
        public DateTime LastMoveTime { get; set; }
        public IReadOnlyBoard BoardState { get; set; } = null!;
    }
}
