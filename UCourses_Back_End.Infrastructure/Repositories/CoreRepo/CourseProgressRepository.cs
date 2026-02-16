using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UCourses_Back_End.Core.DTOs.DashboardDTOs;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Infrastructure.Data;

namespace UCourses_Back_End.Infrastructure.Repositories.CoreRepo
{
    public class CourseProgressRepository : ICourseProgressRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CourseProgressRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> UpdateWatchedDurationAsync(string enrollmentPublicId, string sectionPublicId, int duration)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.PublicId == enrollmentPublicId);

            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.PublicId == sectionPublicId);

            if (enrollment == null || section == null)
                return false;

            var progress = await _context.CourseProgress
                .FirstOrDefaultAsync(p => p.EnrollmentId == enrollment.Id && p.SectionId == section.Id);

            if (progress == null)
                return false;

            progress.WatchedDuration = duration;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkSectionCompletedAsync(string enrollmentPublicId, string sectionPublicId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.PublicId == enrollmentPublicId);

            var section = await _context.Sections
                .FirstOrDefaultAsync(s => s.PublicId == sectionPublicId);

            if (enrollment == null || section == null)
                return false;

            var progress = await _context.CourseProgress
                .FirstOrDefaultAsync(p => p.EnrollmentId == enrollment.Id && p.SectionId == section.Id);

            if (progress == null)
                return false;

            progress.IsCompleted = true;
            progress.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CourseProgressSummaryDTO?> GetCourseProgressAsync(string enrollmentPublicId)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.ProgressRecords)
                    .ThenInclude(p => p.Section)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.PublicId == enrollmentPublicId);

            if (enrollment == null)
                return null;

            var totalSections = enrollment.ProgressRecords.Count;
            var completedSections = enrollment.ProgressRecords.Count(p => p.IsCompleted);
            var progressPercentage = totalSections > 0 ? (decimal)completedSections / totalSections * 100 : 0;

            var sectionProgress = _mapper.Map<List<SectionProgressDTO>>(enrollment.ProgressRecords);

            return new CourseProgressSummaryDTO
            {
                EnrollmentId = enrollment.PublicId,
                CourseId = enrollment.Course.PublicId,
                CourseName = enrollment.Course.Name,
                TotalSections = totalSections,
                CompletedSections = completedSections,
                ProgressPercentage = Math.Round(progressPercentage, 2),
                Sections = sectionProgress
            };
        }
    }
}
