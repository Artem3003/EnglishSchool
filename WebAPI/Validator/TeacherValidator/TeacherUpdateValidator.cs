using System.Text.RegularExpressions;
using demo_english_school.Dtos;
using demo_english_school.Models;
using FluentValidation;

namespace demo_english_school.Validator;

public class TeacherUpdateValidator : AbstractValidator<TeacherUpdateDto>
{
    public TeacherUpdateValidator()
    {
        RuleFor(t => t.Bio).NotNull().MaximumLength(1000).WithMessage("Bio must not exceed 1000 characters.");
        RuleFor(t => t.Qualification).NotNull().MaximumLength(500).WithMessage("Qualification must not exceed 500 characters.");
        RuleFor(t => t.YearsOfExperience).NotNull().InclusiveBetween(1, 50).WithMessage("Years of experience must be between 1 and 50.");
        RuleFor(p => p.Phone)
            .NotEmpty()
            .NotNull().WithMessage("Phone Number is required.")
            .MinimumLength(10).WithMessage("PhoneNumber must not be less than 10 characters.")
            .MaximumLength(20).WithMessage("PhoneNumber must not exceed 50 characters.")
            .Matches(new Regex(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}")).WithMessage("PhoneNumber not valid");
        RuleFor(t => t.Address).NotNull().NotEmpty();
        RuleFor(t => t.User).NotNull();
    }
}