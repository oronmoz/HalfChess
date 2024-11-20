using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalfChess.Core.Domain.Models;

namespace HalfChess.Core.Interfaces
{
    using HalfChess.Core.Domain.Models;
    public interface IGameRepository
    {
        Task<Game> CreateGame(int playerId, int timePerMove);
        Task<Game> GetGame(int gameId);
        Task<IEnumerable<Game>> GetPlayerGames(int playerId);
        Task RecordMove(string gameId, string fromPosition, string toPosition, bool isPlayerMove);
        Task EndGame(string gameId, string result);
    }
}
