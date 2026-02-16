using FluentValidation;
using UCourses_Back_End.Core.DTOs;

namespace UCourses_Back_End.Core.Validators.CoreValidator
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileDTO>
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public UpdateProfileValidator()
        {
            When(x => !string.IsNullOrEmpty(x.FirstName), () =>
            {
                RuleFor(x => x.FirstName)
                    .MinimumLength(2).WithMessage("First name must be at least 2 characters")
                    .MaximumLength(50).WithMessage("First name must not exceed 50 characters");
            });

            When(x => !string.IsNullOrEmpty(x.LastName), () =>
            {
                RuleFor(x => x.LastName)
                    .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
                    .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");
            });

            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                RuleFor(x => x.PhoneNumber)
                    .MinimumLength(10).WithMessage("Phone number must not be less than 10 characters")
                    .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
                    .Matches(@"^\+[1-9]\d{7,14}$")
                    .WithMessage("Phone number must be in E.164 format (e.g. +201001234567)");
            });

            When(x => x.ImageFile != null, () =>
            {
                RuleFor(x => x.ImageFile)
                    .Must(BeAValidImage).WithMessage("Invalid image file. Only jpg, jpeg, png, gif, webp are allowed")
                    .Must(BeWithinSizeLimit).WithMessage("Image size must not exceed 5MB");
            });
        }

        private bool BeAValidImage(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null)
                return true;

            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }

        private bool BeWithinSizeLimit(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null)
                return true;

            return file.Length <= MaxFileSize;
        }
    }
}
