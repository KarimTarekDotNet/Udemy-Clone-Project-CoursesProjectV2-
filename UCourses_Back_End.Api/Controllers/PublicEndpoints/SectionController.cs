using Microsoft.AspNetCore.Mvc;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Api.Controllers.PublicEndpoints
{
    [Route("api/sections")]
    public class SectionController : BaseController
    {
        public SectionController(IUnitOfWork work) : base(work)
        {
        }

        // Public endpoint - Get section details (for preview)
        [HttpGet("{publicId}")]
        public async Task<IActionResult> GetById(string publicId)
        {
            var section = await work.SectionRepository.GetByIdAsync(publicId);

            if (section == null)
                return NotFound(new { message = "Section not found" });

            return Ok(section);
        }

        // Public endpoint - Get sections by course (for preview)
        [HttpGet("course/{coursePublicId}")]
        public async Task<IActionResult> GetByCourse(string coursePublicId, [FromQuery] QueryParams queryParams)
        {
            var (sections, totalCount) = await work.SectionRepository.GetByCourseAsync(coursePublicId, queryParams);

            if (!sections.Any())
                return NotFound(new { message = "No sections found for this course" });

            var response = new PaginatedResponse<SectionDTO>(sections, totalCount, queryParams);
            return Ok(response);
        }
    }
}
