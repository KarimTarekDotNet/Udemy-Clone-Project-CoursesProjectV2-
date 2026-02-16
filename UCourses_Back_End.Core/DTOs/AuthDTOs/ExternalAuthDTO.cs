namespace UCourses_Back_End.Core.DTOs.AuthDTOs
{
    public class ExternalAuthDTO
    {
        public string Provider { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? ProfilePicture { get; set; }
        public string ProviderId { get; set; } = null!;
    }

    // DTO for Google Sign-In from Frontend (with ID Token)
    public class GoogleSignInDTO
    {
        public string IdToken { get; set; } = null!;
        public string? Role { get; set; } = "Student"; // Default to Student
    }
}
