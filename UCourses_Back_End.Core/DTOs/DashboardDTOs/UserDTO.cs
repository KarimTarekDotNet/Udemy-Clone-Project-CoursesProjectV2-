namespace UCourses_Back_End.Core.DTOs.DashboardDTOs
{
    public record AllUsersDTO
    {
        public string PublicId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public string UserType { get; set; } = null!; // "Student", "Instructor", "Admin"
        public DateOnly CreatedAt { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        
        // Instructor-specific fields
        public bool? IsApproved { get; set; }
        public string? InstructorStatus { get; set; }
        public string? DepartmentName { get; set; }
        public int? TotalCourses { get; set; }
        
        // Student-specific fields
        public int? TotalEnrollments { get; set; }
    }
}
