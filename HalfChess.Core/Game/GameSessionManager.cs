using HalfChess.Core.Interfaces;
using System.Collections.Concurrent;

namespace HalfChess.Core.Game
{

    public class GameSessionManager : IGameSessionManager
    {
        private readonly ConcurrentDictionary<Guid, GameSession> _activeSessions = new();
        private readonly ConcurrentDictionary<int, HashSet<Guid>> _playerSessions = new();

        public GameSession CreateSession(int playerId, TimeSpan timePerMove)
        {
            var session = new GameSession(playerId, timePerMove);
            _activeSessions.TryAdd(session.Id, session);

            _playerSessions.AddOrUpdate(
                playerId,
                new HashSet<Guid> { session.Id },
                (_, sessions) =>
                {
                    sessions.Add(session.Id);
                    return sessions;
                });

            return session;
        }

        public GameSession? GetSession(Guid sessionId)
        {
            _activeSessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public bool EndSession(Guid sessionId)
        {
            if (_activeSessions.TryRemove(sessionId, out var session))
            {
                if (_playerSessions.TryGetValue(session.PlayerId, out var sessions))
                {
                    sessions.Remove(sessionId);
                }
                return true;
            }
            return false;
        }

        public IEnumerable<GameSession> GetActiveSessions(int playerId)
        {
            if (_playerSessions.TryGetValue(playerId, out var sessions))
            {
                return sessions.Select(id => _activeSessions[id]).ToList();
            }
            return Enumerable.Empty<GameSession>();
        }
    }
}