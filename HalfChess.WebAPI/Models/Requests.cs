using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
namespace HalfChess.WebAPI.Models
{
    public class GameStartRequest
    {
        [Required]
        public int PlayerId { get; set; }

        [Range(10, 300)]
        public int TimePerMove { get; set; } = 60;
    }

    public class MoveRequest
    {
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string FromPosition { get; set; } = string.Empty;

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string ToPosition { get; set; } = string.Empty;
    }
}

