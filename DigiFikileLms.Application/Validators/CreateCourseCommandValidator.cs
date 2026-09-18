using FluentValidation;
using DigiFikileLms.Application.Features.Courses.Commands;

namespace DigiFikileLms.Application.Validators;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course name is required")
            .MaximumLength(255).WithMessage("Course name cannot exceed 255 characters");

        RuleFor(x => x.FacilitatorId)
            .GreaterThan(0).WithMessage("Facilitator ID is required");
    }
}

