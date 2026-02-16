using Microsoft.AspNetCore.Http;

namespace UCourses_Back_End.Core.DTOs.CoreDTOs
{
    public record CreateSectionDTO(
        string Name,
        string Description,
        IFormFile? VideoFile,
        IFormFile? PdfFile,
        TimeOnly StartAt,
        TimeOnly EndAt,
        DayOfWeek DayOfWeek,
        string CourseId
    );

    public record UpdateSectionDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? VideoFile { get; set; }
        public IFormFile? PdfFile { get; set; }
        public TimeOnly? StartAt { get; set; }
        public TimeOnly? EndAt { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
    }

    public record SectionDTO
    {
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string VideoUrl { get; set; } = null!;
        public string? PdfUrl { get; set; }
        public TimeOnly StartAt { get; set; }
        public TimeOnly EndAt { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string CourseName { get; set; } = null!;
        public DateOnly CreatedAt { get; set; }
    }

    public record SectionDetailsDTO
    {
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string VideoUrl { get; set; } = null!;
        public string? PdfUrl { get; set; }
        public TimeOnly StartAt { get; set; }
        public TimeOnly EndAt { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string CourseId { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public DateOnly CreatedAt { get; set; }
    }
}