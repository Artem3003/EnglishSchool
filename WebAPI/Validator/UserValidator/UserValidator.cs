using demo_english_school.Dtos;
using demo_english_school.Models;
using FluentValidation;

namespace demo_english_school.Validator;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(User => User.Username).NotNull().NotEmpty().WithMessage("Username is required.");
        RuleFor(User => User.Email).NotNull()
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required");
        RuleFor(User => User.FullName).NotNull()
            .NotEmpty().WithMessage("Full Name is required.")
            .MinimumLength(5).WithMessage("Full Name must not be less than 5 characters.");
    }
}