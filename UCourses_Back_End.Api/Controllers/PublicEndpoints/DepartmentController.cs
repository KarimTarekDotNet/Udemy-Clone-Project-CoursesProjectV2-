using Microsoft.AspNetCore.Mvc;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Api.Controllers.PublicEndpoints
{
    [Route("api/departments")]
    public class DepartmentController : BaseController
    {
        public DepartmentController(IUnitOfWork work) : base(work)
        {
        }

        // Public endpoint - Get all departments
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryParams queryParams)
        {
            var (departments, totalCount) = await work.DepartmentRepository.GetAllAsync(queryParams);

            if (!departments.Any())
                return NotFound(new { message = "No departments found" });

            var response = new PaginatedResponse<DepartmentDTO>(departments, totalCount, queryParams);
            return Ok(response);
        }

        // Public endpoint - Get department details
        [HttpGet("{publicId}")]
        public async Task<IActionResult> GetById(string publicId)
        {
            var department = await work.DepartmentRepository.GetByIdAsync(publicId);

            if (department == null)
                return NotFound(new { message = "Department not found" });

            return Ok(department);
        }
    }
}
