namespace HalfChess.Data.Models
{
    public class GameMove
    {
        public int MoveId { get; set; }
        public int GameId { get; set; }
        public string FromPosition { get; set; } = string.Empty;
        public string ToPosition { get; set; } = string.Empty;
        public DateTime MoveTime { get; set; }
        public bool IsPlayerMove { get; set; }

        public virtual Game Game { get; set; } = null!;
    }
}