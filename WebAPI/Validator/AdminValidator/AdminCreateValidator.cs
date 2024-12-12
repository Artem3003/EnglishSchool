using demo_english_school.Models;
using FluentValidation;
using demo_english_school.Dtos;

namespace demo_english_school.Validator;

public class AdminCreateValidator : AbstractValidator<AdminCreateDto>
{
    public AdminCreateValidator()
    {
        RuleFor(Admin => Admin.Id).NotNull();
        RuleFor(Admin => Admin.Role).NotNull().NotEmpty();
        RuleFor(Admin => Admin.UserId).NotNull().WithMessage("User Id is required.");
    }
}