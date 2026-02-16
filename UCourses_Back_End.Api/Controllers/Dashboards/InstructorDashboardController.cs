using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Api.Controllers.Dashboards
{
    [Authorize(Policy = "InstructorOnly")]
    [Route("api/instructor")]
    public class InstructorDashboardController : BaseController
    {
        public InstructorDashboardController(IUnitOfWork work) : base(work)
        {
        }

        private string GetInstructorPublicId()
        {
            return User.FindFirst("PublicId")?.Value ?? throw new UnauthorizedAccessException("Instructor ID not found");
        }

        #region My Courses Management

        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyCourses([FromQuery] QueryParams queryParams)
        {
            var instructorId = GetInstructorPublicId();
            var (courses, totalCount) = await work.CourseRepository.GetByInstructorAsync(instructorId, queryParams);

            if (!courses.Any())
                return NotFound(new { message = "No courses found" });

            var response = new PaginatedResponse<CourseDTO>(courses, totalCount, queryParams);
            return Ok(response);
        }

        [HttpGet("my-courses/{publicId}")]
        public async Task<IActionResult> GetMyCourseById(string publicId)
        {
            var course = await work.CourseRepository.GetByIdAsync(publicId);

            if (course == null)
                return NotFound(new { message = "Course not found" });

            // Verify ownership
            var instructorId = GetInstructorPublicId();
            if (course.InstructorId != instructorId)
                return Forbid();

            return Ok(course);
        }

        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromForm] CreateCourseDTO dto)
        {
            try
            {
                // Override InstructorId from JWT
                var instructorId = GetInstructorPublicId();
                var modifiedDto = new CreateCourseDTO(
                    dto.Name,
                    dto.Description,
                    dto.Price,
                    dto.ImageFile,
                    instructorId,
                    dto.DepartmentId
                );

                var course = await work.CourseRepository.CreateAsync(modifiedDto);

                if (course == null)
                    return BadRequest(new { message = "Failed to create course. Department not found." });

                return CreatedAtAction(nameof(GetMyCourseById), new { publicId = course.PublicId }, new
                {
                    message = "Course created successfully",
                    publicId = course.PublicId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("courses/{publicId}")]
        public async Task<IActionResult> UpdateCourse(string publicId, [FromForm] UpdateCourseDTO dto)
        {
            try
            {
                // Verify ownership
                var course = await work.CourseRepository.GetByIdAsync(publicId);
                if (course == null)
                    return NotFound(new { message = "Course not found" });

                var instructorId = GetInstructorPublicId();
                if (course.InstructorId != instructorId)
                    return Forbid();

                var result = await work.CourseRepository.UpdateAsync(publicId, dto);

                if (!result)
                    return NotFound(new { message = "Course not found" });

                return Ok(new { message = "Course updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("courses/{publicId}")]
        public async Task<IActionResult> DeleteCourse(string publicId)
        {
            // Verify ownership
            var course = await work.CourseRepository.GetByIdAsync(publicId);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            var instructorId = GetInstructorPublicId();
            if (course.InstructorId != instructorId)
                return Forbid();

            var result = await work.CourseRepository.DeleteAsync(publicId);

            if (!result)
                return NotFound(new { message = "Course not found" });

            return Ok(new { message = "Course deleted successfully" });
        }

        [HttpPatch("courses/{publicId}/publish")]
        public async Task<IActionResult> PublishCourse(string publicId)
        {
            // Verify ownership
            var course = await work.CourseRepository.GetByIdAsync(publicId);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            var instructorId = GetInstructorPublicId();
            if (course.InstructorId != instructorId)
                return Forbid();

            var result = await work.CourseRepository.PublishAsync(publicId);

            if (!result)
                return NotFound(new { message = "Course not found" });

            return Ok(new { message = "Course published successfully" });
        }

        [HttpPatch("courses/{publicId}/unpublish")]
        public async Task<IActionResult> UnpublishCourse(string publicId)
        {
            // Verify ownership
            var course = await work.CourseRepository.GetByIdAsync(publicId);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            var instructorId = GetInstructorPublicId();
            if (course.InstructorId != instructorId)
                return Forbid();

            var result = await work.CourseRepository.UnpublishAsync(publicId);

            if (!result)
                return NotFound(new { message = "Course not found" });

            return Ok(new { message = "Course unpublished successfully" });
        }

        #endregion

        #region My Sections Management

        [HttpGet("courses/{coursePublicId}/sections")]
        public async Task<IActionResult> GetCourseSections(string coursePublicId, [FromQuery] QueryParams queryParams)
        {
            // Verify course ownership
            var course = await work.CourseRepository.GetByIdAsync(coursePublicId);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            var instructorId = GetInstructorPublicId();
            if (course.InstructorId != instructorId)
                return Forbid();

            var (sections, totalCount) = await work.SectionRepository.GetByCourseAsync(coursePublicId, queryParams);

            if (!sections.Any())
                return NotFound(new { message = "No sections found" });

            var response = new PaginatedResponse<SectionDTO>(sections, totalCount, queryParams);
            return Ok(response);
        }

        [HttpPost("sections")]
        public async Task<IActionResult> CreateSection([FromForm] CreateSectionDTO dto)
        {
            try
            {
                // Verify course ownership
                var course = await work.CourseRepository.GetByIdAsync(dto.CourseId);
                if (course == null)
                    return NotFound(new { message = "Course not found" });

                var instructorId = GetInstructorPublicId();
                if (course.InstructorId != instructorId)
                    return Forbid();

                var section = await work.SectionRepository.CreateAsync(dto);

                if (section == null)
                    return BadRequest(new { message = "Failed to create section" });

                return Ok(new
                {
                    message = "Section created successfully",
                    publicId = section.PublicId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("sections/{publicId}")]
        public async Task<IActionResult> UpdateSection(string publicId, [FromForm] UpdateSectionDTO dto)
        {
            try
            {
                // Verify section ownership through course
                var section = await work.SectionRepository.GetByIdAsync(publicId);
                if (section == null)
                    return NotFound(new { message = "Section not found" });

                var course = await work.CourseRepository.GetByIdAsync(section.CourseId);
                if (course == null)
                    return NotFound(new { message = "Course not found" });

                var instructorId = GetInstructorPublicId();
                if (course.InstructorId != instructorId)
                    return Forbid();

                var result = await work.SectionRepository.UpdateAsync(publicId, dto);

                if (!result)
                    return NotFound(new { message = "Section not found" });

                return Ok(new { message = "Section updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("sections/{publicId}")]
        public async Task<IActionResult> DeleteSection(string publicId)
        {
            // Verify section ownership through course
            var section = await work.SectionRepository.GetByIdAsync(publicId);
            if (section == null)
                return NotFound(new { message = "Section not found" });

            var course = await work.CourseRepository.GetByIdAsync(section.CourseId);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            var instructorId = GetInstructorPublicId();
            if (course.InstructorId != instructorId)
                return Forbid();

            var result = await work.SectionRepository.DeleteAsync(publicId);

            if (!result)
                return NotFound(new { message = "Section not found" });

            return Ok(new { message = "Section deleted successfully" });
        }

        #endregion

        #region My Students

        [HttpGet("courses/{coursePublicId}/students")]
        public async Task<IActionResult> GetCourseStudents(string coursePublicId)
        {
            // Verify course ownership
            var course = await work.CourseRepository.GetByIdAsync(coursePublicId);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            var instructorId = GetInstructorPublicId();
            if (course.InstructorId != instructorId)
                return Forbid();

            var enrollments = await work.EnrollmentRepository.GetCourseEnrollmentsAsync(coursePublicId);
            return Ok(enrollments);
        }

        #endregion

        #region Earnings & Analytics

        [HttpGet("earnings")]
        public async Task<IActionResult> GetEarnings()
        {
            var instructorId = GetInstructorPublicId();
            var earnings = await work.InstructorRepository.GetInstructorEarningsAsync(instructorId);

            if (earnings == null)
                return NotFound(new { message = "Instructor not found" });

            return Ok(earnings);
        }

        [HttpGet("analytics/{courseId}")]
        public async Task<IActionResult> GetCourseAnalytics(string courseId)
        {
            // Verify course ownership
            var course = await work.CourseRepository.GetByIdAsync(courseId);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            var instructorId = GetInstructorPublicId();
            if (course.InstructorId != instructorId)
                return Forbid();

            var analytics = await work.InstructorRepository.GetCourseAnalyticsAsync(courseId);

            if (analytics == null)
                return NotFound(new { message = "Course not found" });

            return Ok(analytics);
        }

        #endregion
    }
}
