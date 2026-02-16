using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Infrastructure.Data;

namespace UCourses_Back_End.Infrastructure.Repositories.CoreRepo
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EnrollmentRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EnrollmentDTO>> GetAllAsync()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(s => s.AppUser)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<EnrollmentDTO>>(enrollments);
        }

        public async Task<EnrollmentDetailsDTO?> GetByIdAsync(string publicId)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(s => s.AppUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.PublicId == publicId);

            return enrollment == null ? null : _mapper.Map<EnrollmentDetailsDTO>(enrollment);
        }

        public async Task<StudentEnrollmentsDTO?> GetStudentEnrollmentsAsync(string studentPublicId)
        {
            var student = await _context.Students
                .Include(s => s.AppUser)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                            .ThenInclude(i => i.AppUser)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.PublicId == studentPublicId);

            if (student == null)
                return null;

            var courses = student.Enrollments.Select(e => e.Course).ToList();
            var courseDTOs = _mapper.Map<List<CourseDTO>>(courses);

            return new StudentEnrollmentsDTO
            {
                StudentId = student.PublicId,
                StudentName = $"{student.AppUser.FirstName} {student.AppUser.LastName}",
                EnrolledCourses = courseDTOs
            };
        }

        public async Task<CourseEnrollmentsDTO?> GetCourseEnrollmentsAsync(string coursePublicId)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                        .ThenInclude(s => s.AppUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.PublicId == coursePublicId);

            if (course == null)
                return null;

            var enrollmentDTOs = _mapper.Map<List<EnrollmentDTO>>(course.Enrollments);

            return new CourseEnrollmentsDTO
            {
                CourseId = course.PublicId,
                CourseName = course.Name,
                TotalEnrollments = course.Enrollments.Count,
                Enrollments = enrollmentDTOs
            };
        }

        public async Task<Enrollment?> CreateAsync(CreateEnrollmentDTO dto)
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                .FirstOrDefaultAsync(c => c.PublicId == dto.CourseId);

            if (course == null)
            {
                // Course not found
                return null;
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.PublicId == dto.StudentId);

            if (student == null)
            {
                // Student not found
                return null;
            }

            // Check if already enrolled
            var exists = await _context.Enrollments
                .AnyAsync(e => e.CourseId == course.Id && e.StudentId == student.Id);

            if (exists)
            {
                // Already enrolled
                return null;
            }

            var enrollment = new Enrollment
            {
                CourseId = course.Id,
                StudentId = student.Id
            };

            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            // Create CourseProgress records for all sections in the course
            if (course.Sections != null && course.Sections.Any())
            {
                var progressRecords = course.Sections.Select(section => new CourseProgress
                {
                    EnrollmentId = enrollment.Id,
                    SectionId = section.Id,
                    IsCompleted = false,
                    WatchedDuration = 0
                }).ToList();

                await _context.CourseProgress.AddRangeAsync(progressRecords);
                await _context.SaveChangesAsync();
            }

            // Load the course navigation property before returning
            enrollment.Course = course;
            enrollment.Student = student;

            return enrollment;
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.PublicId == publicId);

            if (enrollment == null)
                return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsStudentEnrolledAsync(string studentPublicId, string coursePublicId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AnyAsync(e => e.Student.PublicId == studentPublicId && e.Course.PublicId == coursePublicId);
        }

        public async Task<bool> ExistsAsync(string publicId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.PublicId == publicId);
        }

        public async Task<Enrollment?> GetByStudentAndCourseAsync(string studentPublicId, string coursePublicId)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(s => s.AppUser)
                .FirstOrDefaultAsync(e => e.Student.PublicId == studentPublicId && e.Course.PublicId == coursePublicId);

            return enrollment;
        }
    }
}
