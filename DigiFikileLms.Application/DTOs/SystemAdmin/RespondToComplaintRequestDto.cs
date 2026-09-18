namespace DigiFikileLms.Application.DTOs.SystemAdmin;

public class RespondToComplaintRequestDto
{
    public string Response { get; set; } = string.Empty;
    public string Status { get; set; } = "Resolved"; // InReview, Resolved, Closed
}