using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HalfChess.Core.Game;
using HalfChess.Core.Domain;
namespace HalfChess.WebAPI.Models
{
    public class GameStartResponse
    {
        public Guid GameId { get; set; }
        public IReadOnlyBoard InitialBoard { get; set; } = null!;
        public PieceColor PlayerColor { get; set; }
        public double TimePerMove { get; set; }
    }

    public class MoveResponse
    {
        public bool IsValid { get; set; }
        public string GameState { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public IReadOnlyBoard BoardState { get; set; } = null!;
        public DateTime LastMoveTime { get; set; }
        public PieceColor CurrentTurn { get; set; }
    }

    public class GameStateResponse
    {
        public GameState State { get; set; }
        public PieceColor CurrentTurn { get; set; }
        public DateTime LastMoveTime { get; set; }
        public IReadOnlyBoard BoardState { get; set; } = null!;
    }
}