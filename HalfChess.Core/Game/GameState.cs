namespace HalfChess.Core.Game
{
    public enum GameState
    {
        NotStarted,
        InProgress,
        Check,
        Checkmate,
        Stalemate,
        Timeout,
        Draw,
        Resigned
    }
}