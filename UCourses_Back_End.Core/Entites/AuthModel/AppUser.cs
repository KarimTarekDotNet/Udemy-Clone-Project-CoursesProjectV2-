using Microsoft.AspNetCore.Identity;
using UCourses_Back_End.Core.Enums;

namespace UCourses_Back_End.Core.Entites.AuthModel
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public string ProviderId { get; set; } = Guid.CreateVersion7().ToString();
        public string Provider { get; set; } = "Local";
    }
}