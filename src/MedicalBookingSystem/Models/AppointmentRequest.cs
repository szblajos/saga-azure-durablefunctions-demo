namespace MedicalBookingSystem.Models;

public class AppointmentRequest {
    public required string CalendarId { get; set; }
    public required string PatientId { get; set; }
    public required DateTime AppointmentDate { get; set; }
}