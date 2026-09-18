using FluentValidation;
using DigiFikileLms.Application.Features.Students;

namespace DigiFikileLms.Application.Validators;

public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid student ID");

        When(x => x.Email != null, () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format");
        });
    }
}

