using FluentValidation;
using UCourses_Back_End.Core.DTOs.AuthDTOs;

namespace UCourses_Back_End.Core.Validators.AuthValidator
{
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email).EmailAddress().NotEmpty();

            RuleFor(x => x.FirstName).MinimumLength(2).MaximumLength(50)
                .NotEmpty().WithMessage("Please specify a first name");

            RuleFor(x => x.LastName).MinimumLength(2).MaximumLength(50)
                .NotEmpty().WithMessage("Please specify a first name");

            string usernameRegex = @"^[A-Za-z][A-Za-z0-9_]{7,29}$";
            RuleFor(user => user.Username)
                .MinimumLength(2).MaximumLength(50)
                .Matches(usernameRegex)
                .WithMessage("'Username' is invalid. It must be 8-30 characters long," +
                " start with a letter, and contain only letters, numbers, or underscores.");

            RuleFor(p => p.PhoneNumber)
                .MinimumLength(10).WithMessage("PhoneNumber must not be less than 10 characters.")
                .MaximumLength(20).WithMessage("PhoneNumber must not exceed 20 characters.")
                .Matches(@"^\+[1-9]\d{7,14}$")
                .WithMessage("Phone number must be in E.164 format (e.g. +201001234567)")
                .When(p => !string.IsNullOrWhiteSpace(p.PhoneNumber));


            RuleFor(request => request.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]+").WithMessage("'Password' must contain one or more capital letters.")
                .Matches("[a-z]+").WithMessage("'Password' must contain one or more lowercase letters.")
                .Matches(@"(\d)+").WithMessage("'Password' must contain one or more digits.")
                .Matches(@"[""!@$%^&*(){}:;<>,.?/+\-_=|'[\]~\\]")
                .WithMessage("'Password' must contain one or more special characters.")
                .Matches("(?!.*[£# “”])").WithMessage("'Password' must not contain the following characters £ # “” or spaces.")
                .WithMessage("'Password' contains a word that is not allowed.");

            RuleFor(customer => customer.Password)
                .Equal(customer => customer.ConfirmPassword);


            RuleFor(x => x.City).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Region).NotEmpty().MaximumLength(50);
        }
    }
}