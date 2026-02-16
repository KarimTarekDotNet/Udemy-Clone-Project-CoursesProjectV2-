namespace UCourses_Back_End.Core.Entites.CoreModels
{
    public class CourseProgress : BaseEntity
    {
        public CourseProgress()
        {
            Id = Guid.CreateVersion7();
            PublicId = GeneratePublicId("PROG");
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        public Guid EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        public Guid SectionId { get; set; }
        public Section Section { get; set; } = null!;

        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public int WatchedDuration { get; set; } = 0; // in seconds
    }
}
