using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfChess.Client.Data.Models
{
    public class ClientGame
    {
        public Guid GameId { get; set; }
        public DateTime GameDate { get; set; }
        public string Result { get; set; } = string.Empty;
        public virtual ICollection<ClientMove> Moves { get; set; } = new List<ClientMove>();
    }
}
