namespace UCourses_Back_End.Core.DTOs.AuthDTOs
{
    public record UserProfileDTO
    {
        public string UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public string Role { get; set; } = null!;
    }
}
