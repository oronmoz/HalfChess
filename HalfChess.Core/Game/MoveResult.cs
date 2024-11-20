using HalfChess.Core.Domain;

namespace HalfChess.Core.Game
{
    namespace HalfChess.Core.Game
    {
        public class MoveResult
        {
            public bool IsValid { get; }
            public GameState NewState { get; }
            public string Message { get; }
            public Move? Move { get; }

            public MoveResult(bool isValid, GameState newState, string message, Move? move = null)
            {
                IsValid = isValid;
                NewState = newState;
                Message = message;
                Move = move;
            }

            public static MoveResult Invalid(string message) =>
                new MoveResult(false, GameState.InProgress, message);

            public static MoveResult Success(GameState newState, Move move, string message = "") =>
                new MoveResult(true, newState, message, move);
        }
    }
}