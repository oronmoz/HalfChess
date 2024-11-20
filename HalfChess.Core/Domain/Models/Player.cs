using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfChess.Core.Domain.Models
{
    public class Player
    {
        [Key]
        [Range(1, 1000, ErrorMessage = "ID must be between 1 and 1000")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlayerId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = string.Empty;

        public DateTime RegisteredDate { get; set; }

        public virtual ICollection<Game> Games { get; set; } = new List<Game>();
    }
}