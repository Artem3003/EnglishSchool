using demo_english_school.Models;
using FluentValidation;
using demo_english_school.Dtos;

namespace demo_english_school.Validator;

public class AdminUpdateValidator : AbstractValidator<AdminUpdateDto>
{
    public AdminUpdateValidator()
    {
        RuleFor(Admin => Admin.Id).NotNull();
        RuleFor(Admin => Admin.Role).NotNull().NotEmpty();
    }
}