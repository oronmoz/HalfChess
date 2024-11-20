using HalfChess.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfChess.Core.Interfaces
{
    public interface IGameReplayRepository
    {
        Task SaveMove(Guid gameId, Move move);
        Task<IEnumerable<Move>> GetMoves(Guid gameId);
        Task<IEnumerable<GameReplaySummary>> GetGameReplays();
    }

    public class GameReplaySummary
    {
        public Guid GameId { get; set; }
        public DateTime GameDate { get; set; }
        public string Result { get; set; } = string.Empty;
        public int TotalMoves { get; set; }
    }
}
