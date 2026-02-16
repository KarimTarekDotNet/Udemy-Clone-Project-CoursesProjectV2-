namespace UCourses_Back_End.Core.DTOs.CoreDTOs
{
    public record CreateEnrollmentDTO
    {
        public string CourseId { get; set; } = null!;
        public string StudentId { get; set; } = null!;
    }

    public record EnrollmentDTO
    {
        public string PublicId { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public DateOnly EnrolledAt { get; set; }
    }

    public record EnrollmentDetailsDTO
    {
        public string PublicId { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public decimal CoursePrice { get; set; }
        public string StudentId { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string StudentEmail { get; set; } = null!;
        public DateOnly EnrolledAt { get; set; }
    }

    public record StudentEnrollmentsDTO
    {
        public string StudentId { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public List<CourseDTO> EnrolledCourses { get; set; } = new();
    }

    public record CourseEnrollmentsDTO
    {
        public string CourseId { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public int TotalEnrollments { get; set; }
        public List<EnrollmentDTO> Enrollments { get; set; } = new();
    }
}
