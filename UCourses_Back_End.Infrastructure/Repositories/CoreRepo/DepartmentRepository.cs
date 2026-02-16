using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Core.ModelsView;
using UCourses_Back_End.Infrastructure.Data;

namespace UCourses_Back_End.Infrastructure.Repositories.CoreRepo
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public DepartmentRepository(ApplicationDbContext context, IMapper mapper, IFileService fileService)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<(IEnumerable<DepartmentDTO> departments, int totalCount)> GetAllAsync(QueryParams queryParams)
        {
            var query = _context.Departments
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(queryParams.WordForSearch))
            {
                var searchTerm = $"%{queryParams.WordForSearch}%";
                query = query.Where(d =>
                    EF.Functions.Like(d.Name, searchTerm) ||
                    EF.Functions.Like(d.Description, searchTerm)
                );
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting (always by name for departments)
            query = query.OrderBy(d => d.Name);

            // Pagination
            var departments = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (_mapper.Map<IEnumerable<DepartmentDTO>>(departments), totalCount);
        }

        public async Task<DepartmentDetailsDTO?> GetByIdAsync(string publicId)
        {
            var department = await _context.Departments
                .Include(d => d.Courses)
                .Include(d => d.Instructors)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.PublicId == publicId);

            return department == null ? null : _mapper.Map<DepartmentDetailsDTO>(department);
        }

        public async Task<Department?> CreateAsync(CreateDepartmentDTO dto)
        {
            var department = _mapper.Map<Department>(dto);

            // Upload image if provided
            if (dto.ImageFile != null)
            {
                var imageUrl = await _fileService.UploadDepartmentImageAsync(dto.ImageFile, department.Id.ToString());
                if (imageUrl != null)
                    department.ImageUrl = imageUrl;
            }
            else
            {
                department.ImageUrl = "/Images/Departments/default.jpg";
            }

            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<bool> UpdateAsync(string publicId, UpdateDepartmentDTO dto)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.PublicId == publicId);

            if (department == null)
                return false;

            // Update only if provided
            if (!string.IsNullOrWhiteSpace(dto.Name))
                department.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                department.Description = dto.Description;

            // Upload new image if provided
            if (dto.ImageFile != null)
            {
                await _fileService.DeleteDepartmentImageAsync(department.Id.ToString());
                var imageUrl = await _fileService.UploadDepartmentImageAsync(dto.ImageFile, department.Id.ToString());
                if (imageUrl != null)
                    department.ImageUrl = imageUrl;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.PublicId == publicId);

            if (department == null)
                return false;

            await _fileService.DeleteDepartmentImageAsync(department.Id.ToString());
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string publicId)
        {
            return await _context.Departments
                .AnyAsync(d => d.PublicId == publicId);
        }
    }
}
