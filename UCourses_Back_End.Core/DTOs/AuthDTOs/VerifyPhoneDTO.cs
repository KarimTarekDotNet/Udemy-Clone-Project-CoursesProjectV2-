namespace UCourses_Back_End.Core.DTOs.AuthDTOs
{
    public record VerifyPhoneDTO
    {
        public string Code { get; set; } = null!;
    }
}
