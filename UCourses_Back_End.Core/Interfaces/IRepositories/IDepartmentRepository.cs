using UCourses_Back_End.Core.DTOs.CoreDTOs;
using UCourses_Back_End.Core.Entites.CoreModels;
using UCourses_Back_End.Core.ModelsView;

namespace UCourses_Back_End.Core.Interfaces.IRepositories
{
    public interface IDepartmentRepository
    {
        Task<(IEnumerable<DepartmentDTO> departments, int totalCount)> GetAllAsync(QueryParams queryParams);
        Task<DepartmentDetailsDTO?> GetByIdAsync(string publicId);
        Task<Department?> CreateAsync(CreateDepartmentDTO dto);
        Task<bool> UpdateAsync(string publicId, UpdateDepartmentDTO dto);
        Task<bool> DeleteAsync(string publicId);
        Task<bool> ExistsAsync(string publicId);
    }
}
