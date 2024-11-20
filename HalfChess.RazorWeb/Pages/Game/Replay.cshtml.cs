using HalfChess.Core.Interfaces;
using HalfChess.RazorWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HalfChess.RazorWeb.Pages.Game
{
    public class ReplayModel : PageModel
    {
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<ReplayModel> _logger;

        public int GameId { get; private set; }
        public DateTime GameDate { get; private set; }
        public string WhitePlayerName { get; private set; } = string.Empty;
        public TimeSpan GameDuration { get; private set; }
        public string GameResult { get; private set; } = string.Empty;
        public string ResultBadgeClass { get; private set; } = string.Empty;
        public List<MoveDisplay> Moves { get; private set; } = new();
        public ReplayData ReplayData { get; private set; } = new();

        public ReplayModel(
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            ILogger<ReplayModel> logger)
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
                GameDate = game.StartTime;
                WhitePlayerName = $"{player.FirstName} {player.LastName}";
                GameDuration = game.EndTime!.Value - game.StartTime;
                GameResult = game.Result;
                ResultBadgeClass = GetResultBadgeClass(game.Result);

                // Process moves
                var boardManager = new BoardManager();
                Moves = new List<MoveDisplay>();
                var positions = new List<string>();
                positions.Add(boardManager.GetInitialPosition());

                foreach (var (move, index) in game.Moves.OrderBy(m => m.MoveTime).Select((m, i) => (m, i)))
                {
                    boardManager.MakeMove(move.FromPosition, move.ToPosition);
                    positions.Add(boardManager.GetCurrentPosition());

                    Moves.Add(new MoveDisplay
                    {
                        Index = index,
                        Number = (index / 2) + 1,
                        Notation = $"{move.FromPosition}-{move.ToPosition}",
                        Timestamp = move.MoveTime,
                        IsHighlighted = false
                    });
                }

                ReplayData = new ReplayData
                {
                    InitialPosition = positions[0],
                    Moves = positions.Skip(1).Select((pos, i) => new ReplayMove
                    {
                        Index = i,
                        Position = pos
                    }).ToList()
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading game replay {GameId}", gameId);
                TempData["ErrorMessage"] = "Error loading game replay. Please try again.";
                return RedirectToPage("/Error");
            }
        }

        private string GetResultBadgeClass(string result) => result switch
        {
            "Win" => "bg-success",
            "Loss" => "bg-danger",
            "Draw" => "bg-warning",
            _ => "bg-secondary"
        };
    }

    public class MoveDisplay
    {
        public int Index { get; set; }
        public int Number { get; set; }
        public string Notation { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsHighlighted { get; set; }
    }

    public class ReplayData
    {
        public string InitialPosition { get; set; } = string.Empty;
        public List<ReplayMove> Moves { get; set; } = new();
    }

    public class ReplayMove
    {
        public int Index { get; set; }
        public string Position { get; set; } = string.Empty;
    }
}