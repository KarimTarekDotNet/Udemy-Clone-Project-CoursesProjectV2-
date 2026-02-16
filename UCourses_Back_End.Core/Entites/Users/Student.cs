using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Core.Entites.CoreModels;

namespace UCourses_Back_End.Core.Entites.Users
{
    public class Student : BaseEntity
    {
        public Student()
        {
            Id = Guid.CreateVersion7();
            PublicId = GeneratePublicId("STUD");
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
        public ICollection<Payment>? Payments { get; set; }
    }
}