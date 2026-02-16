namespace UCourses_Back_End.Core.DTOs.DashboardDTOs
{
    public record InstructorEarningsDTO
    {
        public string InstructorId { get; set; } = null!;
        public string InstructorName { get; set; } = null!;
        public decimal TotalEarnings { get; set; }
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public List<CourseEarningDTO> CourseEarnings { get; set; } = new();
    }

    public record CourseEarningDTO
    {
        public string CourseId { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public decimal CoursePrice { get; set; }
        public int EnrolledStudents { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public record CourseAnalyticsDTO
    {
        public string CourseId { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public decimal CoursePrice { get; set; }
        public int TotalStudents { get; set; }
        public int TotalSections { get; set; }
        public int CompletedEnrollments { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<StudentProgressSummary> StudentProgress { get; set; } = new();
    }

    public record StudentProgressSummary
    {
        public string StudentId { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string StudentEmail { get; set; } = null!;
        public DateOnly EnrolledAt { get; set; }
        public int CompletedSections { get; set; }
        public int TotalSections { get; set; }
        public decimal ProgressPercentage { get; set; }
    }
}
