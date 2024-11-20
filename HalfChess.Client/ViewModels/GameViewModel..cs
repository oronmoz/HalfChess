using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HalfChess.Client.Services;
using HalfChess.Core.Domain;
using HalfChess.Core.Game;
using HalfChess.Core.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace HalfChess.Client.ViewModels
{
    public class GameViewModel : ObservableObject
    {
        private readonly IGameService _gameService;
        private readonly IGameReplayRepository _replayRepository;
        private readonly DispatcherTimer _refreshTimer;

        private IReadOnlyBoard? _boardState;
        private string _statusMessage = string.Empty;
        private bool _isPlayerTurn;
        private TimeSpan _remainingMoveTime;

        public ObservableCollection<PieceViewModel> Pieces { get; } = new();
        public Guid? GameId { get; private set; }
        public PieceColor PlayerColor { get; private set; }

        public IAsyncRelayCommand StartGameCommand { get; }
        public IAsyncRelayCommand<Move> MakeMoveCommand { get; }
        public IAsyncRelayCommand ResignCommand { get; }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public TimeSpan RemainingMoveTime
        {
            get => _remainingMoveTime;
            private set => SetProperty(ref _remainingMoveTime, value);
        }

        public GameViewModel(
            IGameService gameService,
            IGameReplayRepository replayRepository)
        {
            _gameService = gameService;
            _replayRepository = replayRepository;

            StartGameCommand = new AsyncRelayCommand(StartGame);
            MakeMoveCommand = new AsyncRelayCommand<Move>(MakeMove);
            ResignCommand = new AsyncRelayCommand(ResignGame);

            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _refreshTimer.Tick += OnRefreshTick;
        }

        private async Task StartGame()
        {
            try
            {
                var response = await _gameService.StartGame(new GameStartRequest
                {
                    PlayerId = App.CurrentPlayerId,
                    TimePerMove = 60
                });

                GameId = response.GameId;
                PlayerColor = response.PlayerColor;
                _boardState = response.InitialBoard;

                UpdateBoardDisplay();
                _refreshTimer.Start();

                StatusMessage = "Game started - " +
                    (PlayerColor == PieceColor.White ? "Your turn" : "Waiting for opponent");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error starting game: {ex.Message}";
            }
        }

        private async Task MakeMove(Move? Move)
        {
            if (Move == null || !GameId.HasValue || !_isPlayerTurn)
                return;

            try
            {
                var response = await _gameService.MakeMove(GameId.Value, new MoveRequest
                {
                    FromPosition = Move.From.ToAlgebraic(),
                    ToPosition = Move.To.ToAlgebraic()
                });

                if (response.IsValid)
                {
                    _boardState = response.BoardState;
                    UpdateBoardDisplay();

                    _isPlayerTurn = response.CurrentTurn == PlayerColor;
                    StatusMessage = $"Game state: {response.GameState}";

                    // Save move for replay
                    await _replayRepository.SaveMove(GameId.Value, Move);
                }
                else
                {
                    StatusMessage = response.Message;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error making move: {ex.Message}";
            }
        }

        private async Task ResignGame()
        {
            if (!GameId.HasValue)
                return;

            try
            {
                await _gameService.ResignGame(GameId.Value);
                _refreshTimer.Stop();
                StatusMessage = "Game resigned";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error resigning game: {ex.Message}";
            }
        }

        private void UpdateBoardDisplay()
        {
            if (_boardState == null) return;

            Pieces.Clear();
            foreach (var pos in _boardState.GetAllPiecePositions(PieceColor.White)
                .Concat(_boardState.GetAllPiecePositions(PieceColor.Black)))
            {
                var piece = _boardState.GetPiece(pos);
                if (piece != null)
                {
                    Pieces.Add(new PieceViewModel(piece, pos));
                }
            }
        }

        private async void OnRefreshTick(object? sender, EventArgs e)
        {
            if (!GameId.HasValue)
                return;

            try
            {
                var state = await _gameService.GetGameState(GameId.Value);

                _isPlayerTurn = state.CurrentTurn == PlayerColor;

                var elapsedTime = DateTime.UtcNow - state.LastMoveTime;
                RemainingMoveTime = TimeSpan.FromSeconds(60) - elapsedTime;

                if (state.State != GameState.InProgress && state.State != GameState.Check)
                {
                    _refreshTimer.Stop();
                    StatusMessage = $"Game ended: {state.State}";
                }
                else
                {
                    StatusMessage = _isPlayerTurn ? "Your turn" : "Opponent's turn";
                }

                _boardState = state.BoardState;
                UpdateBoardDisplay();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating game state: {ex.Message}";
            }
        }
    }
}
