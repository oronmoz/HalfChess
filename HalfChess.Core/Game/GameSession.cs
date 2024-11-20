using HalfChess.Core.Domain;
using HalfChess.Core.Game.HalfChess.Core.Game;
using HalfChess.Core.Interfaces;

namespace HalfChess.Core.Game
{
    public class GameSession
    {
        private readonly IBoard _board;
        private readonly Timer _moveTimer;
        private bool _isTimerRunning;

        public Guid Id { get; } = Guid.NewGuid();
        public int PlayerId { get; }
        public PieceColor PlayerColor { get; }
        public GameState State { get; private set; }
        public PieceColor CurrentTurn { get; private set; }
        public DateTime LastMoveTime { get; private set; }
        public TimeSpan TimePerMove { get; }
        public DateTime? GameStartTime { get; private set; }
        public DateTime? GameEndTime { get; private set; }

        public event EventHandler<GameState>? GameStateChanged;
        public event EventHandler<PieceColor>? TurnChanged;
        public event EventHandler? TimeoutOccurred;

        public GameSession(int playerId, TimeSpan timePerMove, PieceColor playerColor = PieceColor.White)
        {
            PlayerId = playerId;
            TimePerMove = timePerMove;
            PlayerColor = playerColor;
            CurrentTurn = PieceColor.White; // White always starts
            State = GameState.NotStarted;

            _board = new Board();
            _moveTimer = new Timer(OnMoveTimeout, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void StartGame()
        {
            if (State != GameState.NotStarted)
                throw new InvalidOperationException("Game has already started");

            State = GameState.InProgress;
            GameStartTime = DateTime.UtcNow;
            LastMoveTime = DateTime.UtcNow;
            StartMoveTimer();

            GameStateChanged?.Invoke(this, State);
        }

        public MoveResult MakeMove(Position from, Position to)
        {
            if (State != GameState.InProgress && State != GameState.Check)
                return MoveResult.Invalid("Game is not in progress");

            var piece = _board.GetPiece(from);
            if (piece == null)
                return MoveResult.Invalid("No piece at starting position");

            if (piece.Color != CurrentTurn)
                return MoveResult.Invalid("Not your turn");

            if (!piece.CanMoveTo(to, _board))
                return MoveResult.Invalid("Invalid move");

            // Execute the move
            if (!_board.MovePiece(from, to))
                return MoveResult.Invalid("Move execution failed");

            var move = new Move(from, to, piece, _board.GetPiece(to) != null);

            // Update game state
            LastMoveTime = DateTime.UtcNow;
            CurrentTurn = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

            // Reset move timer for next player
            RestartMoveTimer();

            // Check game state
            UpdateGameState();

            TurnChanged?.Invoke(this, CurrentTurn);

            return MoveResult.Success(State, move);
        }

        private void StartMoveTimer()
        {
            _moveTimer.Change(TimePerMove, Timeout.InfiniteTimeSpan);
            _isTimerRunning = true;
        }

        private void StopMoveTimer()
        {
            _moveTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _isTimerRunning = false;
        }

        private void RestartMoveTimer()
        {
            if (_isTimerRunning)
            {
                StopMoveTimer();
                StartMoveTimer();
            }
        }

        private void OnMoveTimeout(object? state)
        {
            State = GameState.Timeout;
            GameEndTime = DateTime.UtcNow;
            StopMoveTimer();

            TimeoutOccurred?.Invoke(this, EventArgs.Empty);
            GameStateChanged?.Invoke(this, State);
        }

        private void UpdateGameState()
        {
            if (_board.IsCheckmate(CurrentTurn))
            {
                State = GameState.Checkmate;
                GameEndTime = DateTime.UtcNow;
                StopMoveTimer();
            }
            else if (_board.IsCheck(CurrentTurn))
            {
                State = GameState.Check;
            }
            else if (_board.IsStalemate(CurrentTurn))
            {
                State = GameState.Stalemate;
                GameEndTime = DateTime.UtcNow;
                StopMoveTimer();
            }

            GameStateChanged?.Invoke(this, State);
        }

        public IReadOnlyBoard GetBoardSnapshot() => new BoardSnapshot(_board);
    }
}