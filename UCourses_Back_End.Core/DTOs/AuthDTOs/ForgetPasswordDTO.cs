namespace UCourses_Back_End.Core.DTOs.AuthDTOs
{
    public record ForgetPasswordDTO
    {
        public string Email { get; set; } = null!;
    }
}
