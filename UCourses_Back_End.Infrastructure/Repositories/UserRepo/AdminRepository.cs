using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UCourses_Back_End.Core.DTOs.DashboardDTOs;
using UCourses_Back_End.Core.Enums.CoreEnum;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Infrastructure.Data;

namespace UCourses_Back_End.Infrastructure.Repositories.UserRepo
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AdminRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AdminStatisticsDTO> GetStatisticsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalStudents = await _context.Students.CountAsync();
            var totalInstructors = await _context.Instructors.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();
            var publishedCourses = await _context.Courses.CountAsync(c => c.Status == CourseStatus.Published);
            var draftCourses = await _context.Courses.CountAsync(c => c.Status == CourseStatus.Draft);
            var totalEnrollments = await _context.Enrollments.CountAsync();

            var totalRevenue = await _context.Enrollments
                .Include(e => e.Course)
                .SumAsync(e => e.Course.Price);

            return new AdminStatisticsDTO
            {
                TotalUsers = totalUsers,
                TotalStudents = totalStudents,
                TotalInstructors = totalInstructors,
                TotalCourses = totalCourses,
                PublishedCourses = publishedCourses,
                DraftCourses = draftCourses,
                TotalEnrollments = totalEnrollments,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<IEnumerable<RecentActivityDTO>> GetRecentActivitiesAsync(int count = 10)
        {
            var activities = new List<RecentActivityDTO>();

            // Recent enrollments
            var recentEnrollments = await _context.Enrollments
                .Include(e => e.Student)
                    .ThenInclude(s => s.AppUser)
                .Include(e => e.Course)
                .OrderByDescending(e => e.CreatedAt)
                .Take(count / 2)
                .AsNoTracking()
                .ToListAsync();

            activities.AddRange(_mapper.Map<List<RecentActivityDTO>>(recentEnrollments));

            // Recent courses
            var recentCourses = await _context.Courses
                .Include(c => c.Instructor)
                    .ThenInclude(i => i.AppUser)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count / 2)
                .AsNoTracking()
                .ToListAsync();

            activities.AddRange(_mapper.Map<List<RecentActivityDTO>>(recentCourses));

            return activities.OrderByDescending(a => a.Timestamp).Take(count);
        }

        public async Task<IEnumerable<AllUsersDTO>> GetAllUsersAsync()
        {
            var users = new List<AllUsersDTO>();

            // Get all students
            var students = await _context.Students
                .Include(s => s.AppUser)
                .Include(s => s.Enrollments)
                .AsNoTracking()
                .Select(s => new AllUsersDTO
                {
                    PublicId = s.PublicId,
                    UserId = s.AppUserId,
                    FirstName = s.AppUser.FirstName,
                    LastName = s.AppUser.LastName,
                    Email = s.AppUser.Email ?? "N/A",
                    PhoneNumber = s.AppUser.PhoneNumber,
                    ImageUrl = s.AppUser.ImageUrl,
                    UserType = "Student",
                    CreatedAt = s.CreatedAt,
                    EmailConfirmed = s.AppUser.EmailConfirmed,
                    PhoneNumberConfirmed = s.AppUser.PhoneNumberConfirmed,
                    TotalEnrollments = s.Enrollments.Count,
                    IsApproved = null,
                    InstructorStatus = null,
                    DepartmentName = null,
                    TotalCourses = null
                })
                .ToListAsync();

            users.AddRange(students);

            // Get all instructors
            var instructors = await _context.Instructors
                .Include(i => i.AppUser)
                .Include(i => i.Department)
                .Include(i => i.Courses)
                .AsNoTracking()
                .Select(i => new AllUsersDTO
                {
                    PublicId = i.PublicId,
                    UserId = i.AppUserId,
                    FirstName = i.AppUser.FirstName,
                    LastName = i.AppUser.LastName,
                    Email = i.AppUser.Email ?? "N/A",
                    PhoneNumber = i.AppUser.PhoneNumber,
                    ImageUrl = i.AppUser.ImageUrl,
                    UserType = "Instructor",
                    CreatedAt = i.CreatedAt,
                    EmailConfirmed = i.AppUser.EmailConfirmed,
                    PhoneNumberConfirmed = i.AppUser.PhoneNumberConfirmed,
                    IsApproved = i.IsApproved,
                    InstructorStatus = i.Status.ToString(),
                    DepartmentName = i.Department != null ? i.Department.Name : null,
                    TotalCourses = i.Courses.Count,
                    TotalEnrollments = null
                })
                .ToListAsync();

            users.AddRange(instructors);

            // Get all admins
            var admins = await _context.Admins
                .Include(a => a.AppUser)
                .AsNoTracking()
                .Select(a => new AllUsersDTO
                {
                    PublicId = a.PublicId,
                    UserId = a.AppUserId,
                    FirstName = a.AppUser.FirstName,
                    LastName = a.AppUser.LastName,
                    Email = a.AppUser.Email ?? "N/A",
                    PhoneNumber = a.AppUser.PhoneNumber,
                    ImageUrl = a.AppUser.ImageUrl,
                    UserType = "Admin",
                    CreatedAt = a.CreatedAt,
                    EmailConfirmed = a.AppUser.EmailConfirmed,
                    PhoneNumberConfirmed = a.AppUser.PhoneNumberConfirmed,
                    IsApproved = null,
                    InstructorStatus = null,
                    DepartmentName = null,
                    TotalCourses = null,
                    TotalEnrollments = null
                })
                .ToListAsync();

            users.AddRange(admins);

            return users.OrderByDescending(u => u.CreatedAt);
        }
    }
}
