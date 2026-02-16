using Microsoft.AspNetCore.Mvc;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Api.Controllers.PublicEndpoints
{
    [Route("api/courses")]
    public class CourseController : BaseController
    {
        public CourseController(IUnitOfWork work) : base(work)
        {
        }

        // Public endpoint - Get all published courses (default route)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryParams queryParams)
        {
            var (courses, totalCount) = await work.CourseRepository.GetPublishedCoursesAsync(queryParams);

            if (!courses.Any())
                return NotFound(new { message = "No published courses found" });

            var response = new PaginatedResponse<CourseDTO>(courses, totalCount, queryParams);
            return Ok(response);
        }

        // Public endpoint - Get all published courses (explicit route)
        [HttpGet("published")]
        public async Task<IActionResult> GetPublished([FromQuery] QueryParams queryParams)
        {
            var (courses, totalCount) = await work.CourseRepository.GetPublishedCoursesAsync(queryParams);

            if (!courses.Any())
                return NotFound(new { message = "No published courses found" });

            var response = new PaginatedResponse<CourseDTO>(courses, totalCount, queryParams);
            return Ok(response);
        }

        // Public endpoint - Get course details
        [HttpGet("{publicId}")]
        public async Task<IActionResult> GetById(string publicId)
        {
            var course = await work.CourseRepository.GetByIdAsync(publicId);

            if (course == null)
                return NotFound(new { message = "Course not found" });

            return Ok(course);
        }

        // Public endpoint - Get courses by department
        [HttpGet("department/{departmentPublicId}")]
        public async Task<IActionResult> GetByDepartment(string departmentPublicId, [FromQuery] QueryParams queryParams)
        {
            var (courses, totalCount) = await work.CourseRepository.GetByDepartmentAsync(departmentPublicId, queryParams);

            if (!courses.Any())
                return NotFound(new { message = "No courses found for this department" });

            var response = new PaginatedResponse<CourseDTO>(courses, totalCount, queryParams);
            return Ok(response);
        }

        // Public endpoint - Get courses by instructor (for browsing)
        [HttpGet("instructor/{instructorPublicId}")]
        public async Task<IActionResult> GetByInstructor(string instructorPublicId, [FromQuery] QueryParams queryParams)
        {
            var (courses, totalCount) = await work.CourseRepository.GetByInstructorAsync(instructorPublicId, queryParams);

            if (!courses.Any())
                return NotFound(new { message = "No courses found for this instructor" });

            var response = new PaginatedResponse<CourseDTO>(courses, totalCount, queryParams);
            return Ok(response);
        }
    }
}
