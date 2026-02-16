using System.ComponentModel.DataAnnotations;

namespace UCourses_Back_End.Core.DTOs.AuthDTOs
{
    public record LoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
}
