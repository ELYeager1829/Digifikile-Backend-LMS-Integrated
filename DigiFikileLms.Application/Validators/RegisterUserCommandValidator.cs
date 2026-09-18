using DigiFikileLms.Application.Features.Auth.Commands;
using FluentValidation;

namespace DigiFikileLms.Application.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required")
            .MaximumLength(100).WithMessage("Surname must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");

        RuleFor(x => x.UserRole)
            .NotEmpty().WithMessage("User role is required")
            .Must(BeValidRole).WithMessage("Invalid user role");
    }

    private bool BeValidRole(string role)
    {
        var validRoles = new[] { "Student", "Facilitator", "Moderator", "SetaAdministrator", "TrainingProvider" };
        return validRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}