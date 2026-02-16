using FluentValidation;
using UCourses_Back_End.Core.DTOs.CoreDTOs;

namespace UCourses_Back_End.Core.Validators.CoreValidator
{
    public class CreateEnrollmentValidator : AbstractValidator<CreateEnrollmentDTO>
    {
        public CreateEnrollmentValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required");

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student ID is required");
        }
    }
}
