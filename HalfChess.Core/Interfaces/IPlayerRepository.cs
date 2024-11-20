using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using HalfChess.Core.Domain.Models;


namespace HalfChess.Core.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player> GetPlayer(int playerId);
        Task<bool> IsPlayerIdTaken(int playerId);
        Task AddPlayer(Player player);
    }
}
