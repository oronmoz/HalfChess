using HalfChess.Core.Domain.Models;

namespace HalfChess.Data.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Result { get; set; } = string.Empty;
        public int TimePerMove { get; set; }

        public virtual Player Player { get; set; } = null!;
        public virtual ICollection<GameMove> Moves { get; set; } = new List<GameMove>();
    }
}
