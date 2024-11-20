using HalfChess.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfChess.Core.Interfaces
{
    public interface IGameSessionManager
    {
        GameSession CreateSession(int playerId, TimeSpan timePerMove);
        GameSession? GetSession(Guid sessionId);
        bool EndSession(Guid sessionId);
        IEnumerable<GameSession> GetActiveSessions(int playerId);
    }
}