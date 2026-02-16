using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Infrastructure.Data;
using UCourses_Back_End.Infrastructure.RealTime;

namespace UCourses_Back_End.Infrastructure.Services.UserServices
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IHubContext<NotificationHub, INotificationHub> hubContext,
            ApplicationDbContext context,
            ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _context = context;
            _logger = logger;
        }

        public async Task NotifyStudentEnrolledAsync(string studentId, string courseName)
        {
            try
            {
                await _hubContext.Clients.User(studentId)
                    .ReceiveNotification($"You have successfully enrolled in {courseName}");

                _logger.LogInformation("Enrollment notification sent to student {StudentId} for course {CourseName}",
                    studentId, courseName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send enrollment notification to student {StudentId}", studentId);
            }
        }

        public async Task NotifyInstructorNewEnrollmentAsync(string instructorId, string studentName, string courseName)
        {
            try
            {
                await _hubContext.Clients.User(instructorId)
                    .ReceiveNotification($"{studentName} has enrolled in your course: {courseName}");

                _logger.LogInformation("New enrollment notification sent to instructor {InstructorId}", instructorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send enrollment notification to instructor {InstructorId}", instructorId);
            }
        }

        public async Task NotifyStudentsNewSectionAsync(string courseId, string sectionName)
        {
            try
            {
                // Get all enrolled students for this course
                var studentIds = await _context.Enrollments
                    .Where(e => e.Course.PublicId == courseId)
                    .Select(e => e.Student.AppUserId)
                    .Distinct()
                    .ToListAsync();

                foreach (var studentId in studentIds)
                {
                    await _hubContext.Clients.User(studentId)
                        .ReceiveNotification($"New section added: {sectionName}");
                }

                _logger.LogInformation("New section notification sent to {Count} students for course {CourseId}",
                    studentIds.Count, courseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send new section notification for course {CourseId}", courseId);
            }
        }

        public async Task NotifyCoursePublishedAsync(string courseId, string courseName)
        {
            try
            {
                // Get instructor
                var instructorId = await _context.Courses
                    .Where(c => c.PublicId == courseId)
                    .Select(c => c.Instructor.AppUserId)
                    .FirstOrDefaultAsync();

                if (instructorId != null)
                {
                    await _hubContext.Clients.User(instructorId)
                        .ReceiveNotification($"Your course '{courseName}' has been published!");
                }

                _logger.LogInformation("Course published notification sent for course {CourseId}", courseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send course published notification for course {CourseId}", courseId);
            }
        }

        public async Task NotifyInstructorApprovedAsync(string instructorId)
        {
            try
            {
                var instructor = await _context.Instructors
                    .Include(i => i.AppUser)
                    .FirstOrDefaultAsync(i => i.PublicId == instructorId);

                if (instructor != null)
                {
                    await _hubContext.Clients.User(instructor.AppUserId)
                        .ReceiveNotification("Congratulations! Your instructor application has been approved.");
                }

                _logger.LogInformation("Instructor approval notification sent to {InstructorId}", instructorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send instructor approval notification to {InstructorId}", instructorId);
            }
        }

        public async Task NotifyInstructorRejectedAsync(string instructorId, string reason)
        {
            try
            {
                var instructor = await _context.Instructors
                    .Include(i => i.AppUser)
                    .FirstOrDefaultAsync(i => i.PublicId == instructorId);

                if (instructor != null)
                {
                    await _hubContext.Clients.User(instructor.AppUserId)
                        .ReceiveNotification($"Your instructor application has been rejected. Reason: {reason}");
                }

                _logger.LogInformation("Instructor rejection notification sent to {InstructorId}", instructorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send instructor rejection notification to {InstructorId}", instructorId);
            }
        }
    }
}