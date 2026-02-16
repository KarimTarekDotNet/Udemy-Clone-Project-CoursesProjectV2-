namespace UCourses_Back_End.Core.ModelsView
{
    public record AuthResult
    {
        public bool Succeeded { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}