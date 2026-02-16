using System.ComponentModel.DataAnnotations.Schema;
using UCourses_Back_End.Core.Entites.AuthModel;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.Enums.CoreEnum;

namespace UCourses_Back_End.Core.Entites.Users
{
    public class Instructor : BaseEntity
    {
        public Instructor()
        {
            Id = Guid.CreateVersion7();
            PublicId = GeneratePublicId("INST");
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            EndContract = CreatedAt.AddYears(1);
        }

        public bool IsApproved { get; set; } = false;
        public DateOnly EndContract { get; set; }
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public InstructorStatus Status { get; set; } = InstructorStatus.Pending;
        public string? RejectionReason { get; set; }

        [NotMapped]
        public bool IsActive => DateOnly.FromDateTime(DateTime.UtcNow) <= EndContract;

        public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
        
        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}