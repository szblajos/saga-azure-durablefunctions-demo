using MedicalBookingSystem.Models;
using MedicalBookingSystem.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MedicalBookingSystem.Activities;

public class NotifySuccessActivity
{
    [Function(nameof(NotifySuccessActivity))]
    public static async Task Run(
        [ActivityTrigger] AppointmentResult result,
        FunctionContext context)
    {
        var logger = context.GetLogger<NotifySuccessActivity>();
        if (context.InstanceServices.GetService(typeof(INotificationService)) is not INotificationService notificationService)
        {
            throw new InvalidOperationException("NotificationService is not registered in the DI container.");
        }

        logger.LogInformation($"Successfully booked appointment for patient {result.PatientId} in calendar {result.CalendarId} at {result.AppointmentDate}. Appointment ID: {result.AppointmentId}");

        await notificationService.SendSuccessNotificationAsync(result);
    }
}

