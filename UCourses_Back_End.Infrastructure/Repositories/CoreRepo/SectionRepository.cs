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
    public class SectionRepository : ISectionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public SectionRepository(ApplicationDbContext context, IMapper mapper, IFileService fileService)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<(IEnumerable<SectionDTO> sections, int totalCount)> GetAllAsync(QueryParams queryParams)
        {
            var query = _context.Sections
                .Include(s => s.Course)
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(queryParams.WordForSearch))
            {
                var searchTerm = $"%{queryParams.WordForSearch}%";
                query = query.Where(s =>
                    EF.Functions.Like(s.Name, searchTerm) ||
                    EF.Functions.Like(s.Description, searchTerm) ||
                    EF.Functions.Like(s.Course.Name, searchTerm)
                );
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting (always by name for sections)
            query = query.OrderBy(s => s.Name);

            // Pagination
            var sections = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (_mapper.Map<IEnumerable<SectionDTO>>(sections), totalCount);
        }

        public async Task<(IEnumerable<SectionDTO> sections, int totalCount)> GetByCourseAsync(string coursePublicId, QueryParams queryParams)
        {
            var query = _context.Sections
                .Include(s => s.Course)
                .Where(s => s.Course.PublicId == coursePublicId)
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(queryParams.WordForSearch))
            {
                var searchTerm = $"%{queryParams.WordForSearch}%";
                query = query.Where(s =>
                    EF.Functions.Like(s.Name, searchTerm) ||
                    EF.Functions.Like(s.Description, searchTerm)
                );
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Sorting (always by name for sections)
            query = query.OrderBy(s => s.Name);

            // Pagination
            var sections = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return (_mapper.Map<IEnumerable<SectionDTO>>(sections), totalCount);
        }

        public async Task<SectionDetailsDTO?> GetByIdAsync(string publicId)
        {
            var section = await _context.Sections
                .Include(s => s.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.PublicId == publicId);

            return section == null ? null : _mapper.Map<SectionDetailsDTO>(section);
        }

        public async Task<Section?> CreateAsync(CreateSectionDTO dto)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.PublicId == dto.CourseId);

            if (course == null)
                return null;

            var section = _mapper.Map<Section>(dto);
            section.CourseId = course.Id;

            // Upload video if provided
            if (dto.VideoFile != null)
            {
                var videoUrl = await _fileService.UploadSectionVideoAsync(dto.VideoFile, section.Id.ToString());
                if (videoUrl != null)
                    section.VideoUrl = videoUrl;
            }
            else
            {
                section.VideoUrl = "/Videos/Sections/default.mp4";
            }

            // Upload PDF if provided
            if (dto.PdfFile != null)
            {
                var pdfUrl = await _fileService.UploadSectionPdfAsync(dto.PdfFile, section.Id.ToString());
                if (pdfUrl != null)
                    section.PdfUrl = pdfUrl;
            }

            await _context.Sections.AddAsync(section);
            await _context.SaveChangesAsync();
            return section;
        }

        public async Task<bool> UpdateAsync(string publicId, UpdateSectionDTO dto)
        {
            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.PublicId == publicId);

            if (section == null)
                return false;

            // Update only if provided
            if (!string.IsNullOrWhiteSpace(dto.Name))
                section.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                section.Description = dto.Description;

            if (dto.StartAt.HasValue)
                section.StartAt = dto.StartAt.Value;

            if (dto.EndAt.HasValue)
                section.EndAt = dto.EndAt.Value;

            if (dto.DayOfWeek.HasValue)
                section.DayOfWeek = dto.DayOfWeek.Value;

            // Upload new video if provided
            if (dto.VideoFile != null)
            {
                await _fileService.DeleteSectionVideoAsync(section.Id.ToString());
                var videoUrl = await _fileService.UploadSectionVideoAsync(dto.VideoFile, section.Id.ToString());
                if (videoUrl != null)
                    section.VideoUrl = videoUrl;
            }

            // Upload new PDF if provided
            if (dto.PdfFile != null)
            {
                await _fileService.DeleteSectionPdfAsync(section.Id.ToString());
                var pdfUrl = await _fileService.UploadSectionPdfAsync(dto.PdfFile, section.Id.ToString());
                if (pdfUrl != null)
                    section.PdfUrl = pdfUrl;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.PublicId == publicId);

            if (section == null)
                return false;

            await _fileService.DeleteSectionVideoAsync(section.Id.ToString());
            await _fileService.DeleteSectionPdfAsync(section.Id.ToString());
            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string publicId)
        {
            return await _context.Sections
                .AnyAsync(s => s.PublicId == publicId);
        }
    }
}
