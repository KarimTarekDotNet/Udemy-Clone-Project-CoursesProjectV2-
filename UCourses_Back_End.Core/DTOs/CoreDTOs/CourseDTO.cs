using Microsoft.AspNetCore.Http;
using UCourses_Back_End.Core.Enums.CoreEnum;

namespace UCourses_Back_End.Core.DTOs.CoreDTOs
{
    public record CreateCourseDTO(
        string Name,
        string Description,
        decimal Price,
        IFormFile? ImageFile,
        string InstructorId,
        string DepartmentId
    );

    public record UpdateCourseDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? DepartmentId { get; set; }
    }

    public record CourseDTO
    {
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public CourseStatus Status { get; set; }
        public string InstructorName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public DateOnly CreatedAt { get; set; }
    }

    public record CourseDetailsDTO
    {
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public CourseStatus Status { get; set; }
        public string InstructorId { get; set; } = null!;
        public string InstructorName { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public int EnrollmentsCount { get; set; }
        public int SectionsCount { get; set; }
        public DateOnly CreatedAt { get; set; }
    }
}
