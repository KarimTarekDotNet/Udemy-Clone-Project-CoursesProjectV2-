using FluentValidation;
using UCourses_Back_End.Core.DTOs.AuthDTOs;

namespace UCourses_Back_End.Core.Validators.AuthValidator
{
    public class VerifyEmailValidator : AbstractValidator<VerifyEmailDTO>
    {
        public VerifyEmailValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Verification code is required")
                .Length(6).WithMessage("Code must be 6 characters")
                .Matches(@"^[A-Z0-9]{6}$").WithMessage("Code must contain only uppercase letters and numbers");
        }
    }
}
