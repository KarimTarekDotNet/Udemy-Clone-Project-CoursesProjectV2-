using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using nClam;
using UCourses_Back_End.Core.Interfaces.IServices;
using UCourses_Back_End.Core.Settings;

namespace UCourses_Back_End.Infrastructure.Services.HelperService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly string[] _allowedVideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".mkv" };
        private readonly string[] _allowedPdfExtensions = { ".pdf" };
        private const long MaxImageSize = 5 * 1024 * 1024; // 5MB
        private const long MaxVideoSize = 500 * 1024 * 1024; // 500MB
        private const long MaxPdfSize = 10 * 1024 * 1024; // 10MB
        private readonly ClamClient? _clam;
        private readonly FileUploadSettings _fileUploadSettings;
        private readonly ILogger<FileService> _logger;

        public FileService(
            IWebHostEnvironment environment, 
            ClamClient? clam, 
            IOptions<FileUploadSettings> fileUploadSettings,
            ILogger<FileService> logger)
        {
            _environment = environment;
            _clam = clam;
            _fileUploadSettings = fileUploadSettings.Value;
            _logger = logger;
        }

        public async Task<string?> UploadProfileImageAsync(IFormFile file, string userId, string username)
        {
            if (file == null || file.Length == 0)
                return null;

            // Validate file size
            if (file.Length > MaxImageSize)
                throw new InvalidOperationException("File size exceeds 5MB limit");

            // Sanitize filename to prevent path traversal
            var originalFileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            
            // Validate file extension
            if (!_allowedImageExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file type. Only images are allowed (jpg, jpeg, png, gif, webp)");

            var allowedMimeTypes = new[]
                {
                    "image/jpeg",
                    "image/png",
                    "image/gif",
                    "image/webp"
                };

            if (!allowedMimeTypes.Contains(file.ContentType))
                throw new InvalidOperationException("Invalid MIME type");

            // Virus scanning with fallback
            await ScanFileForVirusAsync(file);

            // Sanitize username and userId to prevent path traversal
            var sanitizedUsername = SanitizePathComponent(username);
            var sanitizedUserId = SanitizePathComponent(userId);

            // Create user folder: wwwroot/Images/{username}_{userId}
            var userFolderName = $"{sanitizedUsername}_{sanitizedUserId}";
            var userFolderPath = Path.Combine(_environment.WebRootPath, "Images", userFolderName);
            
            // Validate the final path is within WebRootPath
            var fullPath = Path.GetFullPath(userFolderPath);
            var webRootFullPath = Path.GetFullPath(_environment.WebRootPath);
            if (!fullPath.StartsWith(webRootFullPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid path detected");
            }

            // Create directory if not exists
            if (!Directory.Exists(userFolderPath))
            {
                Directory.CreateDirectory(userFolderPath);
            }
            else
            {
                var oldFiles = Directory.GetFiles(userFolderPath)
                                        .Where(f => f.Contains("profile_"));
                foreach (var item in oldFiles)
                {
                    File.Delete(item);
                }
            }

            // Generate unique filename (no user input in filename)
            var fileName = $"profile_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(userFolderPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative URL
            return $"/Images/{userFolderName}/{fileName}";
        }

        public Task<bool> DeleteProfileImageAsync(string userId, string username)
        {
            try
            {
                var userFolderName = $"{username}_{userId}";
                var userFolderPath = Path.Combine(_environment.WebRootPath, "Images", userFolderName);

                if (Directory.Exists(userFolderPath))
                {
                    var files = Directory.GetFiles(userFolderPath);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteUserFolderAsync(string userId, string username)
        {
            try
            {
                var userFolderName = $"{username}_{userId}";
                var userFolderPath = Path.Combine(_environment.WebRootPath, "Images", userFolderName);

                if (Directory.Exists(userFolderPath))
                {
                    Directory.Delete(userFolderPath, true);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<string?> UploadCourseImageAsync(IFormFile file, string courseId)
        {
            return await UploadFileAsync(file, "Images/Courses", courseId, _allowedImageExtensions, MaxImageSize, "course");
        }

        public async Task<string?> UploadDepartmentImageAsync(IFormFile file, string departmentId)
        {
            return await UploadFileAsync(file, "Images/Departments", departmentId, _allowedImageExtensions, MaxImageSize, "department");
        }

        public async Task<string?> UploadSectionVideoAsync(IFormFile file, string sectionId)
        {
            return await UploadFileAsync(file, "Videos/Sections", sectionId, _allowedVideoExtensions, MaxVideoSize, "video");
        }

        public async Task<string?> UploadSectionPdfAsync(IFormFile file, string sectionId)
        {
            return await UploadFileAsync(file, "PDFs/Sections", sectionId, _allowedPdfExtensions, MaxPdfSize, "pdf");
        }

        public Task<bool> DeleteCourseImageAsync(string courseId)
        {
            return DeleteFileAsync("Images/Courses", courseId, "course");
        }

        public Task<bool> DeleteDepartmentImageAsync(string departmentId)
        {
            return DeleteFileAsync("Images/Departments", departmentId, "department");
        }

        public Task<bool> DeleteSectionVideoAsync(string sectionId)
        {
            return DeleteFileAsync("Videos/Sections", sectionId, "video");
        }

        public Task<bool> DeleteSectionPdfAsync(string sectionId)
        {
            return DeleteFileAsync("PDFs/Sections", sectionId, "pdf");
        }

        private async Task<string?> UploadFileAsync(IFormFile file, string folderPath, string entityId, string[] allowedExtensions, long maxSize, string filePrefix)
        {
            if (file == null || file.Length == 0)
                return null;

            if (file.Length > maxSize)
                throw new InvalidOperationException($"File size exceeds {maxSize / (1024 * 1024)}MB limit");

            // Sanitize filename to prevent path traversal
            var originalFileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Invalid file type. Allowed: {string.Join(", ", allowedExtensions)}");

            // MIME type validation
            ValidateMimeType(file.ContentType, extension);

            // Virus scanning with fallback
            await ScanFileForVirusAsync(file);

            // Sanitize entityId to prevent path traversal
            var sanitizedEntityId = SanitizePathComponent(entityId);
            
            var fullFolderPath = Path.Combine(_environment.WebRootPath, folderPath);
            
            // Validate the final path is within WebRootPath
            var fullPath = Path.GetFullPath(fullFolderPath);
            var webRootFullPath = Path.GetFullPath(_environment.WebRootPath);
            if (!fullPath.StartsWith(webRootFullPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid path detected");
            }

            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);

            // Delete old files with same prefix
            var oldFiles = Directory.GetFiles(fullFolderPath)
                                    .Where(f => Path.GetFileName(f).StartsWith($"{filePrefix}_{sanitizedEntityId}_"));
            foreach (var oldFile in oldFiles)
            {
                File.Delete(oldFile);
            }

            // Generate filename (no user input in filename)
            var fileName = $"{filePrefix}_{sanitizedEntityId}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(fullFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folderPath}/{fileName}".Replace("\\", "/");
        }

        // Helper method to scan file for virus with fallback
        private async Task ScanFileForVirusAsync(IFormFile file)
        {
            if (!_fileUploadSettings.EnableVirusScanning)
            {
                _logger.LogWarning("Virus scanning is disabled. File uploaded without scanning.");
                return;
            }

            if (_clam == null)
            {
                _logger.LogWarning("ClamAV client is not configured. File uploaded without virus scanning.");
                return;
            }

            try
            {
                using var scanStream = file.OpenReadStream();
                var result = await _clam.SendAndScanFileAsync(scanStream);

                if (result.Result != ClamScanResults.Clean)
                {
                    _logger.LogWarning("Virus detected in file: {FileName}", file.FileName);
                    throw new InvalidOperationException("File contains a virus");
                }

                _logger.LogInformation("File scanned successfully: {FileName}", file.FileName);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Virus scanning service unavailable for file: {FileName}", file.FileName);
                
                // In production, you might want to fail the upload if scanning is unavailable
                // For now, we log and continue
                _logger.LogWarning("File uploaded without virus scanning due to service unavailability");
            }
        }

        // Helper method to validate MIME type matches extension
        private static void ValidateMimeType(string contentType, string extension)
        {
            var validMimeTypes = new Dictionary<string, string[]>
            {
                { ".jpg", new[] { "image/jpeg" } },
                { ".jpeg", new[] { "image/jpeg" } },
                { ".png", new[] { "image/png" } },
                { ".gif", new[] { "image/gif" } },
                { ".webp", new[] { "image/webp" } },
                { ".mp4", new[] { "video/mp4" } },
                { ".avi", new[] { "video/x-msvideo", "video/avi" } },
                { ".mov", new[] { "video/quicktime" } },
                { ".wmv", new[] { "video/x-ms-wmv" } },
                { ".mkv", new[] { "video/x-matroska" } },
                { ".pdf", new[] { "application/pdf" } }
            };

            if (validMimeTypes.TryGetValue(extension, out var allowedMimeTypes))
            {
                if (!allowedMimeTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"MIME type '{contentType}' does not match file extension '{extension}'");
                }
            }
        }

        private Task<bool> DeleteFileAsync(string folderPath, string entityId, string filePrefix)
        {
            try
            {
                var sanitizedEntityId = SanitizePathComponent(entityId);
                var fullFolderPath = Path.Combine(_environment.WebRootPath, folderPath);

                if (Directory.Exists(fullFolderPath))
                {
                    var files = Directory.GetFiles(fullFolderPath)
                                         .Where(f => Path.GetFileName(f).StartsWith($"{filePrefix}_{sanitizedEntityId}_"));
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        // Helper method to sanitize path components
        private static string SanitizePathComponent(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove any path separators and dangerous characters
            var invalidChars = Path.GetInvalidFileNameChars()
                .Concat(new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' })
                .Distinct()
                .ToArray();

            var sanitized = string.Join("", input.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            
            // Limit length
            if (sanitized.Length > 50)
                sanitized = sanitized.Substring(0, 50);

            return sanitized;
        }
    }
}