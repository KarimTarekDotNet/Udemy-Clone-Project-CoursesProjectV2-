namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface INotificationService
    {
        Task NotifyStudentEnrolledAsync(string studentId, string courseName);
        Task NotifyInstructorNewEnrollmentAsync(string instructorId, string studentName, string courseName);
        Task NotifyStudentsNewSectionAsync(string courseId, string sectionName);
        Task NotifyCoursePublishedAsync(string courseId, string courseName);
        Task NotifyInstructorApprovedAsync(string instructorId);
        Task NotifyInstructorRejectedAsync(string instructorId, string reason);
    }
}
