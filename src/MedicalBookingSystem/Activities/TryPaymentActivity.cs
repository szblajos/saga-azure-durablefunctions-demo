using MedicalBookingSystem.Models;
using MedicalBookingSystem.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MedicalBookingSystem.Activities;

public class TryPaymentActivity
{
    [Function(nameof(TryPaymentActivity))]
    public static Task<bool> Run(
        [ActivityTrigger] AppointmentResult result,
        FunctionContext context)
    {
        var logger = context.GetLogger<TryPaymentActivity>();
        if (context.InstanceServices.GetService(typeof(IPaymentService)) is not IPaymentService paymentService)
        {
            throw new InvalidOperationException("PaymentService is not registered in the DI container.");
        }

        if (result == null)
        {
            throw new ArgumentNullException(nameof(result), "AppointmentResult cannot be null.");
        }
        if (string.IsNullOrEmpty(result.AppointmentId))
        {
            throw new ArgumentException("AppointmentId cannot be null or empty.", nameof(result.AppointmentId));
        }

        logger.LogInformation($"Attempting payment for appointment request in {result.CalendarId} for patient {result.PatientId} at {result.AppointmentDate}");

        // Call PaymentService to process payment and handle response
        return paymentService.TryPaymentAsync(result.AppointmentId, result.PatientId);
    }
}
