using HalfChess.Core.Interfaces;
using HalfChess.Core.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HalfChess.RazorWeb.Pages
{
    public class GameDashboardModel : PageModel
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GameDashboardModel> _logger;

        public string PlayerName { get; set; } = string.Empty;
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public List<GameSummary> ActiveGames { get; set; } = new();
        public List<GameSummary> CompletedGames { get; set; } = new();

        public GameDashboardModel(
            IPlayerRepository playerRepository,
            IGameRepository gameRepository,
            ILogger<GameDashboardModel> logger)
        {
            _playerRepository = playerRepository;
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int playerId)
        {
            try
            {
                var player = await _playerRepository.GetPlayer(playerId);
                if (player == null)
                    return NotFound();

                PlayerName = $"{player.FirstName} {player.LastName}";

                var allGames = await _gameRepository.GetPlayerGames(playerId);

                ActiveGames = allGames
                    .Where(g => g.EndTime == null)
                    .Select(g => new GameSummary(g))
                    .ToList();

                CompletedGames = allGames
                    .Where(g => g.EndTime != null)
                    .Select(g => new GameSummary(g))
                    .ToList();

                GamesPlayed = CompletedGames.Count;
                Wins = CompletedGames.Count(g => g.Result == "Win");
                Losses = CompletedGames.Count(g => g.Result == "Loss");

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard for player {PlayerId}", playerId);
                TempData["ErrorMessage"] = "Error loading dashboard. Please try again.";
                return RedirectToPage("/Error");
            }
        }
    }

    public class GameSummary
    {
        public int GameId { get; set; }
        public DateTime StartTime { get; set; }
        public string State { get; set; }
        public string Result { get; set; }

        public GameSummary(Core.Domain.Models.Game game)
        {
            GameId = game.GameId;
            StartTime = game.StartTime;
            State = game.EndTime.HasValue ? "Completed" : "In Progress";
            Result = game.Result;
        }
    }
}