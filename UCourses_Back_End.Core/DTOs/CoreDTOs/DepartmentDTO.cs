using Microsoft.AspNetCore.Http;

namespace UCourses_Back_End.Core.DTOs.CoreDTOs
{
    public record CreateDepartmentDTO(
        string Name,
        string Description,
        IFormFile? ImageFile
    );

    public record UpdateDepartmentDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public record DepartmentDTO
    {
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateOnly CreatedAt { get; set; }
    }

    public record DepartmentDetailsDTO
    {
        public string PublicId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public DateOnly CreatedAt { get; set; }
        public int CoursesCount { get; set; }
        public int InstructorsCount { get; set; }
    }
}
