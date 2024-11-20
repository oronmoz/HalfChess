using HalfChess.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HalfChess.RazorWeb.Pages.Game
{
    public class ViewModel : PageModel
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<ViewModel> _logger;

        public int GameId { get; private set; }
        public string GameStatus { get; private set; } = string.Empty;
        public string StatusBadgeClass { get; private set; } = string.Empty;
        public DateTime StartTime { get; private set; }
        public string WhitePlayerName { get; private set; } = string.Empty;
        public int TimePerMove { get; private set; }
        public string CurrentTurn { get; private set; } = string.Empty;
        public TimeSpan TimeRemaining { get; private set; }
        public bool IsActive { get; private set; }
        public string PlayerColor { get; private set; } = string.Empty;
        public string InitialPosition { get; private set; } = string.Empty;
        public List<MoveInfo> Moves { get; private set; } = new();

        public ViewModel(
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            ILogger<ViewModel> logger)
        {
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int gameId)
        {
            try
            {
                var game = await _gameRepository.GetGame(gameId);
                if (game == null)
                    return NotFound();

                var player = await _playerRepository.GetPlayer(game.PlayerId);
                if (player == null)
                    return NotFound();

                GameId = gameId;
                StartTime = game.StartTime;
                WhitePlayerName = $"{player.FirstName} {player.LastName}";
                TimePerMove = game.TimePerMove;
                IsActive = game.EndTime == null;

                if (IsActive)
                {
                    GameStatus = "In Progress";
                    StatusBadgeClass = "bg-success";
                    var lastMove = game.Moves.MaxBy(m => m.MoveTime);
                    if (lastMove != null)
                    {
                        TimeRemaining = TimeSpan.FromSeconds(game.TimePerMove) -
                            (DateTime.UtcNow - lastMove.MoveTime);
                    }
                }
                else
                {
                    GameStatus = game.Result;
                    StatusBadgeClass = GetStatusBadgeClass(game.Result);
                }

                Moves = game.Moves
                    .OrderBy(m => m.MoveTime)
                    .Select((m, i) => new MoveInfo
                    {
                        MoveNumber = i + 1,
                        Notation = $"{m.FromPosition}-{m.ToPosition}",
                        Timestamp = m.MoveTime
                    })
                    .ToList();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading game {GameId}", gameId);
                TempData["ErrorMessage"] = "Error loading game. Please try again.";
                return RedirectToPage("/Error");
            }
        }

        private string GetStatusBadgeClass(string result) => result switch
        {
            "Win" => "bg-success",
            "Loss" => "bg-danger",
            "Draw" => "bg-warning",
            "Timeout" => "bg-secondary",
            "Resigned" => "bg-info",
            _ => "bg-secondary"
        };
    }

    public class MoveInfo
    {
        public int MoveNumber { get; set; }
        public string Notation { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
