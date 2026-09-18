using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Feedback : BaseEntity
{
    private Feedback() { }

    public static Feedback Create(
        Moderator moderator,
        Result result,
        Assessment assessment,
        string? title = null,
        string? description = null)
    {
        return new Feedback
        {
            ModeratorId = moderator.Id,
            Moderator = moderator,
            ResultId = result.Id,
            Result = result,
            AssessmentId = assessment.Id,
            Assessment = assessment,
            Title = title,
            Description = description
        };
    }

    public int ModeratorId { get; private set; }
    public int ResultId { get; private set; }
    public int AssessmentId { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }

    public virtual Moderator Moderator { get; private set; } = null!;
    public virtual Result Result { get; private set; } = null!;
    public virtual Assessment Assessment { get; private set; } = null!;
}