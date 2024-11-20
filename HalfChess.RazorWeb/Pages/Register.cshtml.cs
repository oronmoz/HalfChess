using HalfChess.Core.Domain.Models;
using HalfChess.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HalfChess.RazorWeb.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<RegisterModel> _logger;

        [BindProperty]
        public RegisterInput Input { get; set; } = new();
        public List<SelectListItem> Countries { get; } = new();

        public RegisterModel(IPlayerRepository playerRepository, ILogger<RegisterModel> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;

            Countries = new List<SelectListItem>
        {
            new SelectListItem { Value = "USA", Text = "United States" },
            new SelectListItem { Value = "UK", Text = "United Kingdom" },
            new SelectListItem { Value = "IL", Text = "Israel" },
            // Add more countries as needed
        };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                // Check if PlayerId is already taken
                if (await _playerRepository.IsPlayerIdTaken(Input.PlayerId))
                {
                    ModelState.AddModelError("Input.PlayerId",
                        "This Player ID is already taken. Please choose a different number between 1 and 1000.");
                    return Page();
                }

                var player = new Player
                {
                    PlayerId = Input.PlayerId,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    PhoneNumber = Input.PhoneNumber,
                    Country = Input.Country,
                    RegisteredDate = DateTime.UtcNow
                };

                await _playerRepository.AddPlayer(player);

                TempData["SuccessMessage"] = "Registration successful! You can now start playing.";
                return RedirectToPage("/GameDashboard", new { playerId = player.PlayerId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering player");
                ModelState.AddModelError(string.Empty,
                    "Error registering player. Please check your input and try again.");
                return Page();
            }
        }
    }

    public class RegisterInput
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000, ErrorMessage = "ID must be between 1 and 1000")]
        [Display(Name = "Player ID")]
        public int PlayerId { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;
    }
}