namespace UCourses_Back_End.Core.DTOs.AuthDTOs
{
    public record RefreshTokenDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string? UserId { get; set; } // Optional
    }
}
