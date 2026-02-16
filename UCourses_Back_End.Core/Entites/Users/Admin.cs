using UCourses_Back_End.Core.Entites.AuthModel;

namespace UCourses_Back_End.Core.Entites.Users
{
    public class Admin : BaseEntity
    {
        public Admin()
        {
            Id = Guid.CreateVersion7();
            PublicId = GeneratePublicId("ADM");
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
    }
}