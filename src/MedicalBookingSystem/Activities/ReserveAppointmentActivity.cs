using MedicalBookingSystem.Models;
using MedicalBookingSystem.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MedicalBookingSystem.Activities;

public class ReserveAppointmentActivity
{
    [Function(nameof(ReserveAppointmentActivity))]
    public async static Task<AppointmentResult> Run(
        [ActivityTrigger] AppointmentRequest request,
        FunctionContext context)
    {
        var logger = context.GetLogger<ReserveAppointmentActivity>();

        if (context.InstanceServices.GetService(typeof(IAppointmentService)) is not IAppointmentService appointmentService)
        {
            throw new InvalidOperationException("AppointmentService is not registered in the DI container.");
        }

        logger.LogInformation($"Reserving appointment for patient {request.PatientId} at {request.AppointmentDate}");

        // Call AppointmentService to reserve the slot
        var result = await appointmentService.ReserveAsync(request);

        return result;
    }
}
