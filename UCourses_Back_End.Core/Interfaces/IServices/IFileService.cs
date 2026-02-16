using Microsoft.AspNetCore.Http;

namespace UCourses_Back_End.Core.Interfaces.IServices
{
    public interface IFileService
    {
        Task<string?> UploadProfileImageAsync(IFormFile file, string userId, string username);
        Task<bool> DeleteProfileImageAsync(string userId, string username);
        Task<bool> DeleteUserFolderAsync(string userId, string username);
        Task<string?> UploadCourseImageAsync(IFormFile file, string courseId);
        Task<string?> UploadDepartmentImageAsync(IFormFile file, string departmentId);
        Task<string?> UploadSectionVideoAsync(IFormFile file, string sectionId);
        Task<string?> UploadSectionPdfAsync(IFormFile file, string sectionId);
        Task<bool> DeleteCourseImageAsync(string courseId);
        Task<bool> DeleteDepartmentImageAsync(string departmentId);
        Task<bool> DeleteSectionVideoAsync(string sectionId);
        Task<bool> DeleteSectionPdfAsync(string sectionId);
    }
}
