namespace MedicalBookingSystem.Models;

public class AppointmentResult
{
    public string? AppointmentId { get; set; }
    public required string CalendarId { get; set; }
    public required string PatientId { get; set; }
    public required DateTime AppointmentDate { get; set; }
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; } // Optional, only used if IsSuccessful is false
}