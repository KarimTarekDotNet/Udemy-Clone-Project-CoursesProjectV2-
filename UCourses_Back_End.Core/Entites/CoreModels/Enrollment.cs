using UCourses_Back_End.Core.Entites.Users;
using UCourses_Back_End.Core.Enums.CoreEnum;

namespace UCourses_Back_End.Core.Entites.CoreModels
{
    public class Enrollment : BaseEntity
    {
        public Enrollment()
        {
            Id = Guid.CreateVersion7();
            PublicId = GeneratePublicId("ENRL");
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public Payment Payment { get; set; } = null!;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public ICollection<CourseProgress> ProgressRecords { get; set; } = new HashSet<CourseProgress>();
    }
}