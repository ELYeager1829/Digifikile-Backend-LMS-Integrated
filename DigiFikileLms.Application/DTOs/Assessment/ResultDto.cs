namespace DigiFikileLms.Application.DTOs;

public class ResultDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public decimal? Percentage { get; set; }
    public string? Grade { get; set; }
    public DateTime CreatedAt { get; set; }
}