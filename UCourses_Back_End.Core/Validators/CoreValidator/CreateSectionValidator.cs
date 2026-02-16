using FluentValidation;
using UCourses_Back_End.Core.DTOs.CoreDTOs;

namespace UCourses_Back_End.Core.Validators.CoreValidator
{
    public class CreateSectionValidator : AbstractValidator<CreateSectionDTO>
    {
        private readonly string[] _allowedVideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".mkv" };
        private readonly string[] _allowedPdfExtensions = { ".pdf" };
        private const long MaxVideoSize = 500 * 1024 * 1024; // 500MB
        private const long MaxPdfSize = 10 * 1024 * 1024; // 10MB

        public CreateSectionValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Section name is required")
                .MinimumLength(3).WithMessage("Section name must be at least 3 characters")
                .MaximumLength(200).WithMessage("Section name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            When(x => x.VideoFile != null, () =>
            {
                RuleFor(x => x.VideoFile)
                    .Must(BeAValidVideo).WithMessage("Invalid video file. Only mp4, avi, mov, wmv, mkv are allowed")
                    .Must(BeWithinVideoSizeLimit).WithMessage("Video size must not exceed 500MB");
            });

            When(x => x.PdfFile != null, () =>
            {
                RuleFor(x => x.PdfFile)
                    .Must(BeAValidPdf).WithMessage("Invalid PDF file. Only .pdf files are allowed")
                    .Must(BeWithinPdfSizeLimit).WithMessage("PDF size must not exceed 10MB");
            });

            RuleFor(x => x.StartAt)
                .NotEmpty().WithMessage("Start time is required");

            RuleFor(x => x.EndAt)
                .NotEmpty().WithMessage("End time is required")
                .GreaterThan(x => x.StartAt).WithMessage("End time must be after start time");

            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("Invalid day of week");

            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required");
        }

        private bool BeAValidVideo(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null) return true;
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedVideoExtensions.Contains(extension);
        }

        private bool BeWithinVideoSizeLimit(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null) return true;
            return file.Length <= MaxVideoSize;
        }

        private bool BeAValidPdf(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null) return true;
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedPdfExtensions.Contains(extension);
        }

        private bool BeWithinPdfSizeLimit(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null) return true;
            return file.Length <= MaxPdfSize;
        }
    }
}
