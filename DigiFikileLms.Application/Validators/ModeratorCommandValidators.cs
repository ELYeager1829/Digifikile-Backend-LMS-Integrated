using DigiFikileLms.Application.Features.Moderators.Commands;
using FluentValidation;

namespace DigiFikileLms.Application.Validators;

public class CreateModeratorCommandValidator : AbstractValidator<CreateModeratorCommand>
{
    public CreateModeratorCommandValidator()
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

        RuleFor(x => x.StaffNumber)
            .MaximumLength(50).WithMessage("Staff number must not exceed 50 characters");
    }
}

public class UpdateModeratorCommandValidator : AbstractValidator<UpdateModeratorCommand>
{
    public UpdateModeratorCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Surname)
            .MaximumLength(100).WithMessage("Surname must not exceed 100 characters");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");
    }
}