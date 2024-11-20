using HalfChess.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
namespace HalfChess.RazorWeb.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameSessionManager _sessionManager;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IGameSessionManager sessionManager, ILogger<GameHub> logger)
        {
            _sessionManager = sessionManager;
            _logger = logger;
        }

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            _logger.LogInformation("Client {ConnectionId} joined game {GameId}", Context.ConnectionId, gameId);
        }

        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            _logger.LogInformation("Client {ConnectionId} left game {GameId}", Context.ConnectionId, gameId);
        }
    }
}