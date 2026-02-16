using FluentValidation;
using UCourses_Back_End.Core.DTOs.AuthDTOs;

namespace UCourses_Back_End.Core.Validators.AuthValidator
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenDTO>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required")
                .NotNull().WithMessage("Refresh token cannot be null");
        }
    }
}
