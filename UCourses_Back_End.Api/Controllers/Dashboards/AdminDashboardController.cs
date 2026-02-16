using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.DTOs.DashboardDTOs;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Api.Controllers.Dashboards
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/admin")]
    public class AdminDashboardController : BaseController
    {
        public AdminDashboardController(IUnitOfWork work, IAuditLogService auditLog) : base(work, auditLog)
        {
        }

        #region Statistics & Activities

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var statistics = await work.AdminRepository.GetStatisticsAsync();
            return Ok(statistics);
        }

        [HttpGet("recent-activities")]
        public async Task<IActionResult> GetRecentActivities([FromQuery] int count = 10)
        {
            var activities = await work.AdminRepository.GetRecentActivitiesAsync(count);
            return Ok(activities);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await work.AdminRepository.GetAllUsersAsync();

            if (!users.Any())
                return NotFound(new { message = "No users found" });

            return Ok(users);
        }

        #endregion

        #region Course Management

        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourses([FromQuery] QueryParams queryParams)
        {
            var (courses, totalCount) = await work.CourseRepository.GetAllAsync(queryParams);

            if (!courses.Any())
                return NotFound(new { message = "No courses found" });

            var response = new PaginatedResponse<CourseDTO>(courses, totalCount, queryParams);
            return Ok(response);
        }

        [HttpGet("courses/{publicId}")]
        public async Task<IActionResult> GetCourseById(string publicId)
        {
            var course = await work.CourseRepository.GetByIdAsync(publicId);

            if (course == null)
                return NotFound(new { message = "Course not found" });

            return Ok(course);
        }

        [HttpDelete("courses/{publicId}")]
        public async Task<IActionResult> DeleteCourse(string publicId)
        {
            var result = await work.CourseRepository.DeleteAsync(publicId);

            if (!result)
                return NotFound(new { message = "Course not found" });

            // Audit log
            await auditLog!.LogAsync("DELETE_COURSE", "Course", publicId, GetCurrentUserId(), $"Admin deleted course {publicId}");

            return Ok(new { message = "Course deleted successfully" });
        }

        [HttpPatch("courses/{publicId}/publish")]
        public async Task<IActionResult> PublishCourse(string publicId)
        {
            var result = await work.CourseRepository.PublishAsync(publicId);

            if (!result)
                return NotFound(new { message = "Course not found" });

            return Ok(new { message = "Course published successfully" });
        }

        [HttpPatch("courses/{publicId}/unpublish")]
        public async Task<IActionResult> UnpublishCourse(string publicId)
        {
            var result = await work.CourseRepository.UnpublishAsync(publicId);

            if (!result)
                return NotFound(new { message = "Course not found" });

            return Ok(new { message = "Course unpublished successfully" });
        }

        [HttpPatch("courses/{publicId}/archive")]
        public async Task<IActionResult> ArchiveCourse(string publicId)
        {
            var result = await work.CourseRepository.ArchiveAsync(publicId);

            if (!result)
                return NotFound(new { message = "Course not found" });

            return Ok(new { message = "Course archived successfully" });
        }

        #endregion

        #region Department Management

        [HttpGet("departments")]
        public async Task<IActionResult> GetAllDepartments([FromQuery] QueryParams queryParams)
        {
            var (departments, totalCount) = await work.DepartmentRepository.GetAllAsync(queryParams);

            if (!departments.Any())
                return NotFound(new { message = "No departments found" });

            var response = new PaginatedResponse<DepartmentDTO>(departments, totalCount, queryParams);
            return Ok(response);
        }

        [HttpGet("departments/{publicId}")]
        public async Task<IActionResult> GetDepartmentById(string publicId)
        {
            var department = await work.DepartmentRepository.GetByIdAsync(publicId);

            if (department == null)
                return NotFound(new { message = "Department not found" });

            return Ok(department);
        }

        [HttpPost("departments")]
        public async Task<IActionResult> CreateDepartment([FromForm] CreateDepartmentDTO dto)
        {
            try
            {
                var department = await work.DepartmentRepository.CreateAsync(dto);

                if (department == null)
                    return BadRequest(new { message = "Failed to create department" });

                return CreatedAtAction(nameof(GetDepartmentById), new { publicId = department.PublicId }, new
                {
                    message = "Department created successfully",
                    publicId = department.PublicId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("departments/{publicId}")]
        public async Task<IActionResult> UpdateDepartment(string publicId, [FromForm] UpdateDepartmentDTO dto)
        {
            try
            {
                var result = await work.DepartmentRepository.UpdateAsync(publicId, dto);

                if (!result)
                    return NotFound(new { message = "Department not found" });

                return Ok(new { message = "Department updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("departments/{publicId}")]
        public async Task<IActionResult> DeleteDepartment(string publicId)
        {
            var result = await work.DepartmentRepository.DeleteAsync(publicId);

            if (!result)
                return NotFound(new { message = "Department not found" });

            return Ok(new { message = "Department deleted successfully" });
        }

        #endregion

        #region Enrollment Management

        [HttpGet("enrollments")]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var enrollments = await work.EnrollmentRepository.GetAllAsync();
            return Ok(enrollments);
        }

        [HttpGet("enrollments/course/{coursePublicId}")]
        public async Task<IActionResult> GetCourseEnrollments(string coursePublicId)
        {
            var enrollments = await work.EnrollmentRepository.GetCourseEnrollmentsAsync(coursePublicId);
            return Ok(enrollments);
        }

        #endregion

        #region Instructor Management

        [HttpGet("instructors")]
        public async Task<IActionResult> GetAllInstructors()
        {
            var instructors = await work.InstructorRepository.GetAllAsync();

            if (!instructors.Any())
                return NotFound(new { message = "No instructors found" });

            return Ok(instructors);
        }

        [HttpGet("instructors/pending")]
        public async Task<IActionResult> GetPendingInstructors()
        {
            var instructors = await work.InstructorRepository.GetPendingInstructorsAsync();

            if (!instructors.Any())
                return NotFound(new { message = "No pending instructors found" });

            return Ok(instructors);
        }

        [HttpGet("instructors/{publicId}")]
        public async Task<IActionResult> GetInstructorById(string publicId)
        {
            var instructor = await work.InstructorRepository.GetByIdAsync(publicId);

            if (instructor == null)
                return NotFound(new { message = "Instructor not found" });

            return Ok(instructor);
        }

        [HttpPut("instructors/{publicId}/approve")]
        public async Task<IActionResult> ApproveInstructor(string publicId)
        {
            var result = await work.InstructorRepository.ApproveInstructorAsync(publicId);

            if (!result)
                return NotFound(new { message = "Instructor not found" });

            return Ok(new { message = "Instructor approved successfully" });
        }

        [HttpPut("instructors/{publicId}/reject")]
        public async Task<IActionResult> RejectInstructor(string publicId, [FromBody] RejectInstructorDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                return BadRequest(new { message = "Rejection reason is required" });

            var result = await work.InstructorRepository.RejectInstructorAsync(publicId, dto.RejectionReason);

            if (!result)
                return NotFound(new { message = "Instructor not found" });

            return Ok(new { message = "Instructor rejected successfully" });
        }

        #endregion
    }
}
