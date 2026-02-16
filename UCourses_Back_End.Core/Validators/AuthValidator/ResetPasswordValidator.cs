using FluentValidation;
using UCourses_Back_End.Core.DTOs.AuthDTOs;

namespace UCourses_Back_End.Core.Validators.AuthValidator
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Reset code is required")
                .Length(6).WithMessage("Code must be 6 characters")
                .Matches(@"^[A-Z0-9]{6}$").WithMessage("Code must contain only uppercase letters and numbers");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]+").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]+").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"(\d)+").WithMessage("Password must contain at least one digit")
                .Matches(@"[""!@$%^&*(){}:;<>,.?/+\-_=|'[\]~\\]").WithMessage("Password must contain at least one special character")
                .Matches("(?!.*[£# “”])").WithMessage("'Password' must not contain the following characters £ # “” or spaces.");
        }
    }
}
