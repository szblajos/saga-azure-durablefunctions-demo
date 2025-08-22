using MedicalBookingSystem.Models;

namespace MedicalBookingSystem.Services.Mocks;

public class MockNotificationService : INotificationService
{
    public async Task SendSuccessNotificationAsync(AppointmentResult result)
    {
        Console.WriteLine($"[MockNotificationService] Sending success notification: patient: {result.PatientId}, calendar: {result.CalendarId}, time: {result.AppointmentDate}");
        // Simulate sending notification
        await Task.Delay(100); // Simulate some delay
        Console.WriteLine($"[MockNotificationService] Success notification sent for appointment {result.AppointmentId}.");
    }

    public async Task SendFailureNotificationAsync(FailureNotification notification)
    {
        Console.WriteLine($"[MockNotificationService] Sending failure notification: {notification.AppointmentId}, reason: {notification.Reason}");
        // Simulate sending notification
        await Task.Delay(100); // Simulate some delay
        Console.WriteLine($"[MockNotificationService] Failure notification sent for appointment {notification.AppointmentId}.");
    }
}