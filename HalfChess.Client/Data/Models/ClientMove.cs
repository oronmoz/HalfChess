using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfChess.Client.Data.Models
{
    public class ClientMove
    {
        public int MoveId { get; set; }
        public Guid GameId { get; set; }
        public string FromPosition { get; set; } = string.Empty;
        public string ToPosition { get; set; } = string.Empty;
        public string PieceType { get; set; } = string.Empty;
        public bool IsCapture { get; set; }
        public DateTime MoveTime { get; set; }

        public virtual ClientGame Game { get; set; } = null!;
    }
}