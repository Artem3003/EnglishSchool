using demo_english_school.Dtos;
using demo_english_school.Models;
using FluentValidation;
using System.Text.RegularExpressions;

namespace demo_english_school.Validator;

public class StudentCreateValidator : AbstractValidator<StudentCreateDto>
{
    public StudentCreateValidator()
    {
        RuleFor(Student => Student.Id).NotNull().WithMessage("Student Id is required.");
        RuleFor(Student => Student.DateOfBirth).NotNull().WithMessage("Date of birth is required.");
        RuleFor(p => p.Phone)
            .NotEmpty()
            .NotNull().WithMessage("Phone Number is required.")
            .MinimumLength(10).WithMessage("PhoneNumber must not be less than 10 characters.")
            .MaximumLength(20).WithMessage("PhoneNumber must not exceed 50 characters.")
            .Matches(new Regex(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}")).WithMessage("PhoneNumber not valid");
        RuleFor(Student => Student.Address).NotNull().NotEmpty().WithMessage("Address is required.");
        RuleFor(Student => Student.UserId).NotNull().WithMessage("User Id is required.");
    }
}