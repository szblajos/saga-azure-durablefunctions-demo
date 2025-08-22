using MedicalBookingSystem.Models;
using MedicalBookingSystem.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MedicalBookingSystem.Activities;

public class ReleaseAppointmentActivity
{
    [Function(nameof(ReleaseAppointmentActivity))]
    public async Task<bool> Run(
        [ActivityTrigger] AppointmentResult result,
        FunctionContext context)
    {
        var logger = context.GetLogger<ReleaseAppointmentActivity>();
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

        logger.LogInformation($"Releasing appointment with ID: {result.AppointmentId}");

        // Call AppointmentService to release the slot
        var isReleased = await appointmentService.ReleaseAsync(result.AppointmentId);
        if (isReleased)
        {
            logger.LogInformation($"Appointment {result.AppointmentId} released successfully.");
        }
        else
        {
            logger.LogWarning($"Failed to release appointment {result.AppointmentId}.");
        }

        return isReleased;
    }
}