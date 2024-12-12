using demo_english_school.Dtos;
using demo_english_school.Models;
using FluentValidation;

namespace demo_english_school.Validator;

public class UserCreateValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateValidator()
    {
        RuleFor(User => User.Username).NotNull().NotEmpty().WithMessage("Username is required.");
        RuleFor(p => p.Password).NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
            .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        RuleFor(User => User.Email).NotNull()
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required");
        RuleFor(User => User.FullName).NotNull()
            .NotEmpty().WithMessage("Full Name is required.")
            .MinimumLength(5).WithMessage("Full Name must not be less than 5 characters.");
    }
}