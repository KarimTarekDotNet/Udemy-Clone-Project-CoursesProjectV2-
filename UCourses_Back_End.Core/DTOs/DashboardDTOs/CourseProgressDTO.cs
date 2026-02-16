namespace UCourses_Back_End.Core.DTOs.DashboardDTOs
{
    public record UpdateProgressDTO(
        string EnrollmentId,
        string SectionId,
        int WatchedDuration
    );

    public record CompleteSectionDTO(
        string EnrollmentId,
        string SectionId
    );

    public record SectionProgressDTO
    {
        public string SectionId { get; set; } = null!;
        public string SectionName { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int WatchedDuration { get; set; }
    }

    public record CourseProgressSummaryDTO
    {
        public string EnrollmentId { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public int TotalSections { get; set; }
        public int CompletedSections { get; set; }
        public decimal ProgressPercentage { get; set; }
        public List<SectionProgressDTO> Sections { get; set; } = new();
    }
}
