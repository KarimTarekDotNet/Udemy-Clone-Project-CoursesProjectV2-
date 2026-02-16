using FluentValidation;
using UCourses_Back_End.Core.DTOs.AuthDTOs;

namespace UCourses_Back_End.Core.Validators.AuthValidator
{
    public class GoogleSignInValidator : AbstractValidator<GoogleSignInDTO>
    {
        public GoogleSignInValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("Google ID Token is required")
                .MinimumLength(100).WithMessage("Invalid Google ID Token format");

            RuleFor(x => x.Role)
                .Must(role => role == null || 
                             role.Equals("Student", StringComparison.OrdinalIgnoreCase) || 
                             role.Equals("Instructor", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Role must be either 'Student' or 'Instructor' (case-insensitive)");
        }
    }
}
