namespace MedicalBookingSystem.Models;

public class FailureNotification {
    public string? AppointmentId { get; set; }
    public required DateTime AppointmentDate { get; set; }
    public required string Reason { get; set; }
}