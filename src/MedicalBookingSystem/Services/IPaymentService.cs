namespace MedicalBookingSystem.Services;

public interface IPaymentService
{
    Task<bool> TryPaymentAsync(string appointmentId, string patientId);
}