using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.DTOs.DashboardDTOs;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Api.Controllers.Dashboards
{
    [Authorize(Policy = "StudentOnly")]
    [Route("api/student")]
    public class StudentDashboardController : BaseController
    {
        public StudentDashboardController(IUnitOfWork work) : base(work)
        {
        }

        private string GetStudentPublicId()
        {
            return User.FindFirst("PublicId")?.Value ?? throw new UnauthorizedAccessException("Student ID not found");
        }

        #region My Enrolled Courses

        [HttpGet("my-courses")]
        public async Task<IActionResult> GetMyEnrolledCourses()
        {
            var studentId = GetStudentPublicId();
            var enrollments = await work.EnrollmentRepository.GetStudentEnrollmentsAsync(studentId);

            if (enrollments == null)
                return NotFound(new { message = "No enrolled courses found" });

            return Ok(enrollments);
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollInCourse([FromBody] CreateEnrollmentDTO dto)
        {
            try
            {
                // Override StudentId from JWT
                var studentId = GetStudentPublicId();
                var modifiedDto = new CreateEnrollmentDTO
                {
                    CourseId = dto.CourseId,
                    StudentId = studentId
                };

                var enrollment = await work.EnrollmentRepository.CreateAsync(modifiedDto);

                if (enrollment == null)
                    return BadRequest(new { message = "Failed to enroll. Course or Student not found, or already enrolled." });

                return Ok(new
                {
                    message = "Enrolled successfully",
                    publicId = enrollment.PublicId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("enrollments/{publicId}")]
        public async Task<IActionResult> CancelEnrollment(string publicId)
        {
            // Verify ownership
            var enrollment = await work.EnrollmentRepository.GetByIdAsync(publicId);
            if (enrollment == null)
                return NotFound(new { message = "Enrollment not found" });

            var studentId = GetStudentPublicId();
            if (enrollment.StudentId != studentId)
                return Forbid();

            var result = await work.EnrollmentRepository.DeleteAsync(publicId);

            if (!result)
                return NotFound(new { message = "Enrollment not found" });

            return Ok(new { message = "Enrollment cancelled successfully" });
        }

        #endregion

        #region Course Content Access

        [HttpGet("courses/{coursePublicId}")]
        public async Task<IActionResult> GetEnrolledCourseDetails(string coursePublicId)
        {
            var studentId = GetStudentPublicId();

            // Check if student is enrolled
            var isEnrolled = await work.EnrollmentRepository.IsStudentEnrolledAsync(studentId, coursePublicId);
            if (!isEnrolled)
                return Forbid();

            var course = await work.CourseRepository.GetByIdAsync(coursePublicId);

            if (course == null)
                return NotFound(new { message = "Course not found" });

            return Ok(course);
        }

        [HttpGet("courses/{coursePublicId}/sections")]
        public async Task<IActionResult> GetCourseSections(string coursePublicId, [FromQuery] QueryParams queryParams)
        {
            var studentId = GetStudentPublicId();

            // Check if student is enrolled
            var isEnrolled = await work.EnrollmentRepository.IsStudentEnrolledAsync(studentId, coursePublicId);
            if (!isEnrolled)
                return Forbid();

            var (sections, totalCount) = await work.SectionRepository.GetByCourseAsync(coursePublicId, queryParams);

            if (!sections.Any())
                return NotFound(new { message = "No sections found" });

            var response = new PaginatedResponse<SectionDTO>(sections, totalCount, queryParams);
            return Ok(response);
        }

        [HttpGet("sections/{publicId}")]
        public async Task<IActionResult> GetSectionDetails(string publicId)
        {
            var section = await work.SectionRepository.GetByIdAsync(publicId);

            if (section == null)
                return NotFound(new { message = "Section not found" });

            var studentId = GetStudentPublicId();

            // Check if student is enrolled in the course
            var isEnrolled = await work.EnrollmentRepository.IsStudentEnrolledAsync(studentId, section.CourseId);
            if (!isEnrolled)
                return Forbid();

            return Ok(section);
        }

        #endregion

        #region Browse Courses (for enrollment)

        [HttpGet("browse/courses")]
        public async Task<IActionResult> BrowsePublishedCourses([FromQuery] QueryParams queryParams)
        {
            var (courses, totalCount) = await work.CourseRepository.GetPublishedCoursesAsync(queryParams);

            if (!courses.Any())
                return NotFound(new { message = "No published courses found" });

            var response = new PaginatedResponse<CourseDTO>(courses, totalCount, queryParams);
            return Ok(response);
        }

        [HttpGet("browse/courses/{publicId}")]
        public async Task<IActionResult> GetPublishedCourseDetails(string publicId)
        {
            var course = await work.CourseRepository.GetByIdAsync(publicId);

            if (course == null)
                return NotFound(new { message = "Course not found" });

            return Ok(course);
        }

        [HttpGet("browse/departments")]
        public async Task<IActionResult> BrowseDepartments([FromQuery] QueryParams queryParams)
        {
            var (departments, totalCount) = await work.DepartmentRepository.GetAllAsync(queryParams);

            if (!departments.Any())
                return NotFound(new { message = "No departments found" });

            var response = new PaginatedResponse<DepartmentDTO>(departments, totalCount, queryParams);
            return Ok(response);
        }

        [HttpGet("browse/instructors/{instructorPublicId}/courses")]
        public async Task<IActionResult> GetInstructorCourses(string instructorPublicId, [FromQuery] QueryParams queryParams)
        {
            var (courses, totalCount) = await work.CourseRepository.GetByInstructorAsync(instructorPublicId, queryParams);

            if (!courses.Any())
                return NotFound(new { message = "No courses found for this instructor" });

            var response = new PaginatedResponse<CourseDTO>(courses, totalCount, queryParams);
            return Ok(response);
        }

        #endregion

        #region Course Progress Tracking

        [HttpGet("progress/{enrollmentPublicId}")]
        public async Task<IActionResult> GetCourseProgress(string enrollmentPublicId)
        {
            // Verify ownership
            var enrollment = await work.EnrollmentRepository.GetByIdAsync(enrollmentPublicId);
            if (enrollment == null)
                return NotFound(new { message = "Enrollment not found" });

            var studentId = GetStudentPublicId();
            if (enrollment.StudentId != studentId)
                return Forbid();

            var progress = await work.CourseProgressRepository.GetCourseProgressAsync(enrollmentPublicId);

            if (progress == null)
                return NotFound(new { message = "Progress not found" });

            return Ok(progress);
        }

        [HttpPost("progress/update")]
        public async Task<IActionResult> UpdateWatchedDuration([FromBody] UpdateProgressDTO dto)
        {
            // Verify ownership
            var enrollment = await work.EnrollmentRepository.GetByIdAsync(dto.EnrollmentId);
            if (enrollment == null)
                return NotFound(new { message = "Enrollment not found" });

            var studentId = GetStudentPublicId();
            if (enrollment.StudentId != studentId)
                return Forbid();

            var result = await work.CourseProgressRepository.UpdateWatchedDurationAsync(
                dto.EnrollmentId, 
                dto.SectionId, 
                dto.WatchedDuration
            );

            if (!result)
                return BadRequest(new { message = "Failed to update progress" });

            return Ok(new { message = "Progress updated successfully" });
        }

        [HttpPost("progress/complete")]
        public async Task<IActionResult> MarkSectionCompleted([FromBody] CompleteSectionDTO dto)
        {
            // Verify ownership
            var enrollment = await work.EnrollmentRepository.GetByIdAsync(dto.EnrollmentId);
            if (enrollment == null)
                return NotFound(new { message = "Enrollment not found" });

            var studentId = GetStudentPublicId();
            if (enrollment.StudentId != studentId)
                return Forbid();

            var result = await work.CourseProgressRepository.MarkSectionCompletedAsync(
                dto.EnrollmentId, 
                dto.SectionId
            );

            if (!result)
                return BadRequest(new { message = "Failed to mark section as completed" });

            return Ok(new { message = "Section marked as completed" });
        }

        #endregion
    }
}