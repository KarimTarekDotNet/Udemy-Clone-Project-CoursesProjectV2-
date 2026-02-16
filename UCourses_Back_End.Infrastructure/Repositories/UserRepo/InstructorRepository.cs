using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UCourses_Back_End.Core.DTOs.DashboardDTOs;
using UCourses_Back_End.Core.Entites.Users;
using UCourses_Back_End.Core.Enums.CoreEnum;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Infrastructure.Data;

namespace UCourses_Back_End.Infrastructure.Repositories.UserRepo
{
    public class InstructorRepository : IInstructorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;

        public InstructorRepository(ApplicationDbContext context, IMapper mapper, IMailService mailService)
        {
            _context = context;
            _mapper = mapper;
            _mailService = mailService;
        }

        public async Task<IEnumerable<InstructorDTO>> GetAllAsync()
        {
            var instructors = await _context.Instructors
                .Include(i => i.AppUser)
                .Include(i => i.Department)
                .Include(i => i.Courses)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<InstructorDTO>>(instructors);
        }

        public async Task<IEnumerable<InstructorDTO>> GetPendingInstructorsAsync()
        {
            var instructors = await _context.Instructors
                .Include(i => i.AppUser)
                .Include(i => i.Department)
                .Include(i => i.Courses)
                .Where(i => i.Status == InstructorStatus.Pending)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<InstructorDTO>>(instructors);
        }

        public async Task<InstructorDetailsDTO?> GetByIdAsync(string publicId)
        {
            var instructor = await _context.Instructors
                .Include(i => i.AppUser)
                .Include(i => i.Department)
                .Include(i => i.Courses)
                    .ThenInclude(c => c.Enrollments)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.PublicId == publicId);

            return instructor == null ? null : _mapper.Map<InstructorDetailsDTO>(instructor);
        }

        public async Task<Instructor?> GetByUserIdAsync(string userId)
        {
            return await _context.Instructors
                .Include(i => i.AppUser)
                .Include(i => i.Department)
                .FirstOrDefaultAsync(i => i.AppUserId == userId);
        }

        public async Task<bool> ApproveInstructorAsync(string publicId)
        {
            var instructor = await _context.Instructors
                .Include(i => i.AppUser)
                .FirstOrDefaultAsync(i => i.PublicId == publicId);

            if (instructor == null)
                return false;

            instructor.Status = InstructorStatus.Approved;
            instructor.IsApproved = true;
            instructor.RejectionReason = null;

            await _context.SaveChangesAsync();

            // Send approval email
            if (instructor.AppUser != null && !string.IsNullOrEmpty(instructor.AppUser.Email))
            {
                var subject = "Instructor Application Approved";
                var body = $@"
                    <h2>Congratulations!</h2>
                    <p>Dear {instructor.AppUser.FirstName} {instructor.AppUser.LastName},</p>
                    <p>Your instructor application has been approved. You can now start creating courses on our platform.</p>
                    <p>Welcome to our team of instructors!</p>
                    <br>
                    <p>Best regards,<br>UCourses Team</p>
                ";

                await _mailService.SendEmail(instructor.AppUser.Email, subject, body);
            }

            return true;
        }

        public async Task<bool> RejectInstructorAsync(string publicId, string rejectionReason)
        {
            var instructor = await _context.Instructors
                .Include(i => i.AppUser)
                .FirstOrDefaultAsync(i => i.PublicId == publicId);

            if (instructor == null)
                return false;

            instructor.Status = InstructorStatus.Rejected;
            instructor.IsApproved = false;
            instructor.RejectionReason = rejectionReason;

            await _context.SaveChangesAsync();

            // Send rejection email
            if (instructor.AppUser != null && !string.IsNullOrEmpty(instructor.AppUser.Email))
            {
                var subject = "Instructor Application Update";
                var body = $@"
                    <h2>Application Status Update</h2>
                    <p>Dear {instructor.AppUser.FirstName} {instructor.AppUser.LastName},</p>
                    <p>Thank you for your interest in becoming an instructor on our platform.</p>
                    <p>Unfortunately, we are unable to approve your application at this time.</p>
                    <p><strong>Reason:</strong> {rejectionReason}</p>
                    <p>If you have any questions or would like to reapply in the future, please contact our support team.</p>
                    <br>
                    <p>Best regards,<br>UCourses Team</p>
                ";

                await _mailService.SendEmail(instructor.AppUser.Email, subject, body);
            }

            return true;
        }

        public async Task<bool> ExistsAsync(string publicId)
        {
            return await _context.Instructors
                .AnyAsync(i => i.PublicId == publicId);
        }

        public async Task<InstructorEarningsDTO?> GetInstructorEarningsAsync(string instructorPublicId)
        {
            var instructor = await _context.Instructors
                .Include(i => i.AppUser)
                .Include(i => i.Courses)
                    .ThenInclude(c => c.Enrollments)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.PublicId == instructorPublicId);

            if (instructor == null)
                return null;

            var courseEarnings = _mapper.Map<List<CourseEarningDTO>>(instructor.Courses);

            var totalEarnings = courseEarnings.Sum(ce => ce.TotalRevenue);
            var totalStudents = instructor.Courses.SelectMany(c => c.Enrollments).Select(e => e.StudentId).Distinct().Count();

            return new InstructorEarningsDTO
            {
                InstructorId = instructor.PublicId,
                InstructorName = $"{instructor.AppUser.FirstName} {instructor.AppUser.LastName}",
                TotalEarnings = totalEarnings,
                TotalCourses = instructor.Courses.Count,
                TotalStudents = totalStudents,
                CourseEarnings = courseEarnings
            };
        }

        public async Task<CourseAnalyticsDTO?> GetCourseAnalyticsAsync(string coursePublicId)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                        .ThenInclude(s => s.AppUser)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.ProgressRecords)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.PublicId == coursePublicId);

            if (course == null)
                return null;

            var totalSections = course.Sections?.Count ?? 0;
            var totalStudents = course.Enrollments.Count;
            var completedEnrollments = course.Enrollments.Count(e => 
                e.ProgressRecords.Count > 0 && 
                e.ProgressRecords.All(p => p.IsCompleted)
            );
            var completionRate = totalStudents > 0 ? (decimal)completedEnrollments / totalStudents * 100 : 0;

            var studentProgress = _mapper.Map<List<StudentProgressSummary>>(course.Enrollments);

            return new CourseAnalyticsDTO
            {
                CourseId = course.PublicId,
                CourseName = course.Name,
                CoursePrice = course.Price,
                TotalStudents = totalStudents,
                TotalSections = totalSections,
                CompletedEnrollments = completedEnrollments,
                CompletionRate = Math.Round(completionRate, 2),
                TotalRevenue = course.Price * totalStudents,
                StudentProgress = studentProgress
            };
        }
    }
}
