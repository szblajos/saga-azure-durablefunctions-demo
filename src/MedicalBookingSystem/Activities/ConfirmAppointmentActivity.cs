using MedicalBookingSystem.Models;
using MedicalBookingSystem.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MedicalBookingSystem.Activities;

public class ConfirmAppointmentActivity
{
    [Function(nameof(ConfirmAppointmentActivity))]
    public static async Task Run([ActivityTrigger] AppointmentResult result, FunctionContext context)
    {
        var logger = context.GetLogger<ConfirmAppointmentActivity>();
        if (context.InstanceServices.GetService(typeof(IAppointmentService)) is not IAppointmentService appointmentService)
        {
            throw new InvalidOperationException("AppointmentService is not registered in the DI container.");
        }

        if (result == null)
        {
            throw new ArgumentNullException(nameof(result), "AppointmentResult cannot be null.");
        }
        if (string.IsNullOrEmpty(result.AppointmentId))
        {
            throw new ArgumentException("AppointmentId cannot be null or empty.", nameof(result.AppointmentId));
        }

        logger.LogInformation($"Confirming appointment for patient {result.PatientId} in calendar {result.CalendarId} at {result.AppointmentDate} with ID: {result.AppointmentId}");

        var isConfirmed = await appointmentService.ConfirmAsync(result.AppointmentId);
        if (isConfirmed)
        {
            logger.LogInformation($"Appointment {result.AppointmentId} confirmed successfully.");
        }
        else
        {
            logger.LogWarning($"Failed to confirm appointment {result.AppointmentId}.");
        }
    }
}