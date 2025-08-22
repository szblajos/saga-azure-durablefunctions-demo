using MedicalBookingSystem.Models;

namespace MedicalBookingSystem.Services;

public interface INotificationService
{
    Task SendSuccessNotificationAsync(AppointmentResult result);
    Task SendFailureNotificationAsync(FailureNotification notification);
}
