using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.Enums.CoreEnum;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Core.ModelsView;
using UCourses_Back_End.Infrastructure.Data;

namespace UCourses_Back_End.Infrastructure.Repositories.CoreRepo
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public CourseRepository(ApplicationDbContext context, IMapper mapper, IFileService fileService)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<(IEnumerable<CourseDTO> courses, int totalCount)> GetAllAsync(QueryParams queryParams)
        {
            var query = _context.Courses
                .Include(c => c.Instructor)
                    .ThenInclude(i => i.AppUser)
                .Include(c => c.Department)
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(queryParams.WordForSearch))
            {
                var searchTerm = $"%{queryParams.WordForSearch}%";
                query = query.Where(c =>
                    EF.Functions.Like(c.Name, searchTerm) ||
                    EF.Functions.Like(c.Description, searchTerm) ||
                    EF.Functions.Like(c.Department.Name, searchTerm)
                );
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            query = queryParams.SortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(c => c.Price),
                "price_desc" => query.OrderByDescending(c => c.Price),
                _ => query.OrderBy(c => c.Name)
            };

            // Pagination
            var courses = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (_mapper.Map<IEnumerable<CourseDTO>>(courses), totalCount);
        }

        public async Task<(IEnumerable<CourseDTO> courses, int totalCount)> GetPublishedCoursesAsync(QueryParams queryParams)
        {
            var query = _context.Courses
                .Include(c => c.Instructor)
                    .ThenInclude(i => i.AppUser)
                .Include(c => c.Department)
                .Where(c => c.Status == CourseStatus.Published)
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(queryParams.WordForSearch))
            {
                var searchTerm = $"%{queryParams.WordForSearch}%";
                query = query.Where(c =>
                    EF.Functions.Like(c.Name, searchTerm) ||
                    EF.Functions.Like(c.Description, searchTerm) ||
                    EF.Functions.Like(c.Department.Name, searchTerm)
                );
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            query = queryParams.SortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(c => c.Price),
                "price_desc" => query.OrderByDescending(c => c.Price),
                _ => query.OrderBy(c => c.Name)
            };

            // Pagination
            var courses = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (_mapper.Map<IEnumerable<CourseDTO>>(courses), totalCount);
        }

        public async Task<(IEnumerable<CourseDTO> courses, int totalCount)> GetByDepartmentAsync(string departmentPublicId, QueryParams queryParams)
        {
            var query = _context.Courses
                .Include(c => c.Instructor)
                    .ThenInclude(i => i.AppUser)
                .Include(c => c.Department)
                .Where(c => c.Department.PublicId == departmentPublicId)
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(queryParams.WordForSearch))
            {
                var searchTerm = $"%{queryParams.WordForSearch}%";
                query = query.Where(c =>
                    EF.Functions.Like(c.Name, searchTerm) ||
                    EF.Functions.Like(c.Description, searchTerm)
                );
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            query = queryParams.SortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(c => c.Price),
                "price_desc" => query.OrderByDescending(c => c.Price),
                _ => query.OrderBy(c => c.Name)
            };

            // Pagination
            var courses = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (_mapper.Map<IEnumerable<CourseDTO>>(courses), totalCount);
        }

        public async Task<(IEnumerable<CourseDTO> courses, int totalCount)> GetByInstructorAsync(string instructorPublicId, QueryParams queryParams)
        {
            var query = _context.Courses
                .Include(c => c.Instructor)
                    .ThenInclude(i => i.AppUser)
                .Include(c => c.Department)
                .Where(c => c.Instructor.PublicId == instructorPublicId)
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(queryParams.WordForSearch))
            {
                var searchTerm = $"%{queryParams.WordForSearch}%";
                query = query.Where(c =>
                    EF.Functions.Like(c.Name, searchTerm) ||
                    EF.Functions.Like(c.Description, searchTerm) ||
                    EF.Functions.Like(c.Department.Name, searchTerm)
                );
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting
            query = queryParams.SortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(c => c.Price),
                "price_desc" => query.OrderByDescending(c => c.Price),
                _ => query.OrderBy(c => c.Name)
            };

            // Pagination
            var courses = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (_mapper.Map<IEnumerable<CourseDTO>>(courses), totalCount);
        }

        public async Task<CourseDetailsDTO?> GetByIdAsync(string publicId)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                    .ThenInclude(i => i.AppUser)
                .Include(c => c.Department)
                .Include(c => c.Enrollments)
                .Include(c => c.Sections)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.PublicId == publicId);

            return course == null ? null : _mapper.Map<CourseDetailsDTO>(course);
        }

        public async Task<Course?> CreateAsync(CreateCourseDTO dto)
        {
            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.PublicId == dto.InstructorId);

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.PublicId == dto.DepartmentId);

            if (instructor == null || department == null)
                return null;

            var course = _mapper.Map<Course>(dto);
            course.InstructorId = instructor.Id;
            course.DepartmentId = department.Id;

            // Upload image if provided
            if (dto.ImageFile != null)
            {
                var imageUrl = await _fileService.UploadCourseImageAsync(dto.ImageFile, course.Id.ToString());
                if (imageUrl != null)
                    course.ImageUrl = imageUrl;
            }
            else
            {
                course.ImageUrl = "/Images/Courses/default.jpg";
            }

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> UpdateAsync(string publicId, UpdateCourseDTO dto)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.PublicId == publicId);

            if (course == null)
                return false;

            // Update only if provided
            if (!string.IsNullOrWhiteSpace(dto.Name))
                course.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                course.Description = dto.Description;

            if (dto.Price.HasValue && dto.Price.Value > 0)
                course.Price = dto.Price.Value;

            // Update department if provided
            if (!string.IsNullOrWhiteSpace(dto.DepartmentId))
            {
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.PublicId == dto.DepartmentId);

                if (department != null)
                    course.DepartmentId = department.Id;
            }

            // Upload new image if provided
            if (dto.ImageFile != null)
            {
                await _fileService.DeleteCourseImageAsync(course.Id.ToString());
                var imageUrl = await _fileService.UploadCourseImageAsync(dto.ImageFile, course.Id.ToString());
                if (imageUrl != null)
                    course.ImageUrl = imageUrl;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.PublicId == publicId);

            if (course == null)
                return false;

            await _fileService.DeleteCourseImageAsync(course.Id.ToString());
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string publicId)
        {
            return await _context.Courses
                .AnyAsync(c => c.PublicId == publicId);
        }

        public async Task<bool> PublishAsync(string publicId)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.PublicId == publicId);

            if (course == null)
                return false;

            course.Status = CourseStatus.Published;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnpublishAsync(string publicId)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.PublicId == publicId);

            if (course == null)
                return false;

            course.Status = CourseStatus.Draft;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ArchiveAsync(string publicId)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.PublicId == publicId);

            if (course == null)
                return false;

            course.Status = CourseStatus.Archived;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
