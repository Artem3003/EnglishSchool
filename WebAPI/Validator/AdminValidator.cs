using demo_english_school.Models;
using FluentValidation;

namespace demo_english_school.Validator;

public class AdminValidator : AbstractValidator<Admin>
{
    public AdminValidator()
    {
        RuleFor(Admin => Admin.Id).NotNull();
        RuleFor(Admin => Admin.Role).NotNull().NotEmpty();
        RuleFor(Admin => Admin.UserId).NotNull().WithMessage("User Id is required.");
    }
}