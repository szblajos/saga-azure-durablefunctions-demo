using MedicalBookingSystem.Models;

namespace MedicalBookingSystem.Services.Mocks;

public class MockAppointmentService : IAppointmentService
{
    public async Task<AppointmentResult> ReserveAsync(AppointmentRequest appointmentRequest)
    {

        // simulate reservation process
        Console.WriteLine($"[MockAppointmentService] Reserve appointment. Calendar: {appointmentRequest.CalendarId}, Patient: {appointmentRequest.PatientId}, Date: {appointmentRequest.AppointmentDate}");

        await Task.Delay(1000);
        return new AppointmentResult
        {
            IsSuccessful = true,
            AppointmentId = Guid.NewGuid().ToString(),
            CalendarId = appointmentRequest.CalendarId,
            PatientId = appointmentRequest.PatientId,
            AppointmentDate = appointmentRequest.AppointmentDate
        };
    }

    public async Task<bool> ConfirmAsync(string appointmentId)
    {
        // simulate confirmation process
        Console.WriteLine($"[MockAppointmentService] Confirm appointment: {appointmentId}");
        await Task.Delay(1000);
        return true;
    }

    public async Task<bool> ReleaseAsync(string appointmentId)
    {
        Console.WriteLine($"[MockAppointmentService] Release appointment: {appointmentId}");
        await Task.Delay(1000);
        return true;
    }
}
