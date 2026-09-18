using DigiFikileLms.Domain.Common; 

namespace DigiFikileLms.Domain.Entities;

public class Result : BaseEntity
{
    private readonly List<Certificate> _certificates = new();
    private readonly List<Feedback> _feedbacks = new();

    private Result() { }

    public static Result Create(
        Student student,
        decimal? percentage = null,
        string? grade = null)      //? > Why is  the grade parameter nullable? Because not all results will have a grade assigned at the time of creation. Some results may only have a percentage, and the grade may be determined later based on the percentage or other criteria. Making it nullable allows for flexibility in creating Result objects without requiring a grade to be provided upfront. ,
    {
        return new Result
        {
            StudentId = student.Id,
            Student = student,
            Percentage = percentage,
            Grade = grade
        };
    }

    public int StudentId { get; private set; }
    public decimal? Percentage { get; private set; }
    public string? Grade { get; private set; }  // so c# , like Kotlin , meaning just defining it as a dataybe without the questionnmark will expet something to be there 

    public virtual Student Student { get; private set; } = null!; // is a dataype you will be using if you want something to be overwritten  ! 
    public IReadOnlyCollection<Certificate> Certificates => _certificates.AsReadOnly();
    public IReadOnlyCollection<Feedback> Feedbacks => _feedbacks.AsReadOnly();

    public void UpdateResult(decimal percentage, string grade)
    {
        Percentage = percentage;
        Grade = grade;
        UpdateTimestamp();
    }
}