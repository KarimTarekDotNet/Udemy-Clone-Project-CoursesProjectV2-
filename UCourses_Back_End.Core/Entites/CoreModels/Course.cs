using UCourses_Back_End.Core.Entites.Users;
using UCourses_Back_End.Core.Enums.CoreEnum;

namespace UCourses_Back_End.Core.Entites.CoreModels
{
    public class Course : BaseEntity
    {
        public Course()
        {
            Id = Guid.CreateVersion7();
            PublicId = GeneratePublicId("CRS");
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            Status = CourseStatus.Draft; // Default status
        }

        public decimal Price { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public CourseStatus Status { get; set; }

        public Guid InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;
        
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
        
        public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
        public ICollection<Section>? Sections { get; set; }
    }
    public class Section : BaseEntity
    {
        public Section()
        {
            Id = Guid.CreateVersion7();
            PublicId = GeneratePublicId("SEC");
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string VideoUrl { get; set; } = null!;
        public string? PdfUrl { get; set; }
        public TimeOnly StartAt { get; set; }
        public TimeOnly EndAt { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}