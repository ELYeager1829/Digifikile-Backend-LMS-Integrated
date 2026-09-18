using DigiFikileLms.Application.Features.Permissions.Commands;
using FluentValidation;

namespace DigiFikileLms.Application.Validators;

public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Permission code is required")
            .MaximumLength(100).WithMessage("Permission code must not exceed 100 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required")
            .MaximumLength(150).WithMessage("Permission name must not exceed 150 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}

public class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Permission code is required")
            .MaximumLength(100).WithMessage("Permission code must not exceed 100 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required")
            .MaximumLength(150).WithMessage("Permission name must not exceed 150 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}