using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCourses_Back_End.Core.Interfaces.IRepositories;

namespace UCourses_Back_End.Api.Controllers.PublicEndpoints
{
    [Route("api/enrollments")]
    public class EnrollmentController : BaseController
    {
        public EnrollmentController(IUnitOfWork work) : base(work)
        {
        }

        // Requires authentication - Check if student is enrolled in a course
        [HttpGet("check/{studentPublicId}/{coursePublicId}")]
        [Authorize]
        public async Task<IActionResult> CheckEnrollment(string studentPublicId, string coursePublicId)
        {
            var isEnrolled = await work.EnrollmentRepository.IsStudentEnrolledAsync(studentPublicId, coursePublicId);
            return Ok(new { isEnrolled });
        }
    }
}
