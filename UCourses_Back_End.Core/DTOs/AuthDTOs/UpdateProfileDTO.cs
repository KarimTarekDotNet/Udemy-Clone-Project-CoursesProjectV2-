using Microsoft.AspNetCore.Http;

namespace UCourses_Back_End.Core.DTOs
{
    public record UpdateProfileDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
