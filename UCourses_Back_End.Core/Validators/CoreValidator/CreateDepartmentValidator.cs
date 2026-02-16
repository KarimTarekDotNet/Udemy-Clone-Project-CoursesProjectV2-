using FluentValidation;
using UCourses_Back_End.Core.DTOs.CoreDTOs;

namespace UCourses_Back_End.Core.Validators.CoreValidator
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDTO>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public CreateDepartmentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required")
                .MinimumLength(3).WithMessage("Department name must be at least 3 characters")
                .MaximumLength(100).WithMessage("Department name must not exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            When(x => x.ImageFile != null, () =>
            {
                RuleFor(x => x.ImageFile)
                    .Must(BeAValidImage).WithMessage("Invalid image file. Only jpg, jpeg, png, gif, webp are allowed")
                    .Must(BeWithinSizeLimit).WithMessage("Image size must not exceed 5MB");
            });
        }

        private bool BeAValidImage(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null) return true;
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }

        private bool BeWithinSizeLimit(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null) return true;
            return file.Length <= MaxFileSize;
        }
    }
}
