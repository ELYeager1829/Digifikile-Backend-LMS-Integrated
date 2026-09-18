using FluentValidation;
using DigiFikileLms.Application.Features.Assessments.Commands;

namespace DigiFikileLms.Application.Validators;

public class CreateAssessmentCommandValidator : AbstractValidator<CreateAssessmentCommand>
{
    public CreateAssessmentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Assessment title is required")
            .MaximumLength(255).WithMessage("Title cannot exceed 255 characters");

        RuleFor(x => x.ModuleId)
            .GreaterThan(0).WithMessage("Module ID is required");

        RuleFor(x => x.FacilitatorId)
            .GreaterThan(0).WithMessage("Facilitator ID is required");

        RuleFor(x => x.AssessmentType)
            .NotEmpty().WithMessage("Assessment type is required")
            .MaximumLength(50).WithMessage("Assessment type cannot exceed 50 characters");
    }
}

