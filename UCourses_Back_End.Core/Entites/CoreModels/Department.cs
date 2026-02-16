using UCourses_Back_End.Core.Entites.Users;

namespace UCourses_Back_End.Core.Entites.CoreModels
{
    public class Department : BaseEntity
    {
        public Department()
        {
            Id = Guid.CreateVersion7();
            PublicId = GeneratePublicId("DEPT");
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        
        public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
        public ICollection<Instructor> Instructors { get; set; } = new HashSet<Instructor>();
    }
}