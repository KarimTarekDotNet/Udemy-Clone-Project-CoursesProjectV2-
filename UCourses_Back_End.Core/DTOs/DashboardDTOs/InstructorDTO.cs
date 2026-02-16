using UCourses_Back_End.Core.Enums.CoreEnum;

namespace UCourses_Back_End.Core.DTOs.DashboardDTOs
{
    public record InstructorDTO
    {
        public string PublicId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsApproved { get; set; }
        public InstructorStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateOnly EndContract { get; set; }
        public bool IsActive { get; set; }
        public string? DepartmentName { get; set; }
        public int CoursesCount { get; set; }
        public DateOnly CreatedAt { get; set; }
    }

    public record InstructorDetailsDTO
    {
        public string PublicId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsApproved { get; set; }
        public InstructorStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateOnly EndContract { get; set; }
        public bool IsActive { get; set; }
        public string? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int CoursesCount { get; set; }
        public int StudentsCount { get; set; }
        public DateOnly CreatedAt { get; set; }
    }

    public record RejectInstructorDTO(
        string RejectionReason
    );
}
