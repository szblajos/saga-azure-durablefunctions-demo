using MedicalBookingSystem.Models;

namespace MedicalBookingSystem.Services;

public interface IAppointmentService
{
    Task<AppointmentResult> ReserveAsync(AppointmentRequest appointmentRequest);
    Task<bool> ConfirmAsync(string appointmentId);
    Task<bool> ReleaseAsync(string appointmentId);
}