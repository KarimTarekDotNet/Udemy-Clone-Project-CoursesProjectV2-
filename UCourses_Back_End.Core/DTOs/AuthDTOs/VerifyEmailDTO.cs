namespace UCourses_Back_End.Core.DTOs.AuthDTOs
{
    public record VerifyEmailDTO
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
