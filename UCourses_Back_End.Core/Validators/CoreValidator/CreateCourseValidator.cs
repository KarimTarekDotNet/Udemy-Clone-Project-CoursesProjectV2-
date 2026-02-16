using FluentValidation;
using UCourses_Back_End.Core.DTOs.CoreDTOs;

namespace UCourses_Back_End.Core.Validators.CoreValidator
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseDTO>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public CreateCourseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .MinimumLength(3).WithMessage("Course name must be at least 3 characters")
                .MaximumLength(200).WithMessage("Course name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters")
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(100000).WithMessage("Price must not exceed 100,000");

            When(x => x.ImageFile != null, () =>
            {
                RuleFor(x => x.ImageFile)
                    .Must(BeAValidImage).WithMessage("Invalid image file. Only jpg, jpeg, png, gif, webp are allowed")
                    .Must(BeWithinSizeLimit).WithMessage("Image size must not exceed 5MB");
            });

            RuleFor(x => x.InstructorId)
                .NotEmpty().WithMessage("Instructor ID is required");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required");
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
