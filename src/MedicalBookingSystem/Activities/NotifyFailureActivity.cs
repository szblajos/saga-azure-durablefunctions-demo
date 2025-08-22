
using MedicalBookingSystem.Models;
using MedicalBookingSystem.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MedicalBookingSystem.Activities;

public class NotifyFailureActivity
{
    [Function(nameof(NotifyFailureActivity))]
    public static async Task Run(
        [ActivityTrigger] AppointmentResult result,
        FunctionContext context)
    {
        var logger = context.GetLogger<NotifyFailureActivity>();
        if (context.InstanceServices.GetService(typeof(INotificationService)) is not INotificationService notificationService)
        {
            throw new InvalidOperationException("NotificationService is not registered in the DI container.");
        }
        
        logger.LogError($"Failed to book appointment for patient {result.PatientId} in calendar {result.CalendarId} at {result.AppointmentDate}. Reason: {result.FailureReason}");

        await notificationService.SendFailureNotificationAsync(
            new FailureNotification
            {
                AppointmentId = result.AppointmentId,
                AppointmentDate = result.AppointmentDate,
                Reason = result.FailureReason ?? "Unknown error"
            });
    }
}
