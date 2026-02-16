using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Core.Interfaces.IRepositories
{
    public interface ISectionRepository
    {
        Task<(IEnumerable<SectionDTO> sections, int totalCount)> GetAllAsync(QueryParams queryParams);
        Task<(IEnumerable<SectionDTO> sections, int totalCount)> GetByCourseAsync(string coursePublicId, QueryParams queryParams);
        Task<SectionDetailsDTO?> GetByIdAsync(string publicId);
        Task<Section?> CreateAsync(CreateSectionDTO dto);
        Task<bool> UpdateAsync(string publicId, UpdateSectionDTO dto);
        Task<bool> DeleteAsync(string publicId);
        Task<bool> ExistsAsync(string publicId);
    }
}
