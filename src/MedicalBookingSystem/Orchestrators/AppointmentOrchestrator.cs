using MedicalBookingSystem.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace MedicalBookingSystem.Orchestrators;

public class AppointmentOrchestrator
{
    [Function(nameof(AppointmentOrchestrator))]
    public static async Task<AppointmentResult> Run([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var logger = context.CreateReplaySafeLogger(nameof(AppointmentOrchestrator));
        var request = context.GetInput<AppointmentRequest>();
        if (request == null)
        {
            logger.LogError("Invalid appointment request.");
            return new AppointmentResult
            {
                IsSuccessful = false,
                FailureReason = "Invalid appointment request.",
                AppointmentId = null,
                CalendarId = string.Empty,
                PatientId = string.Empty,
                AppointmentDate = DateTime.MinValue
            };
        }

        var appointmentReservation = await context.CallActivityAsync<AppointmentResult>("ReserveAppointmentActivity", request);
        if (!appointmentReservation.IsSuccessful)
        {
            logger.LogError($"Failed to reserve appointment. Cause: {appointmentReservation.FailureReason}");
            return appointmentReservation;
        }

        var deadline = context.CurrentUtcDateTime.AddMinutes(10);
        var paymentSuccess = false;
        var attempts = 0;

        while (context.CurrentUtcDateTime < deadline && !paymentSuccess && attempts < 3)
        {
            paymentSuccess = await context.CallActivityAsync<bool>("TryPaymentActivity", appointmentReservation);
            if (!paymentSuccess)
            {
                attempts++;

                logger.LogWarning($"Payment attempt {attempts} failed.");
                await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(10), CancellationToken.None);
            }
        }

        if (paymentSuccess)
        {
            await context.CallActivityAsync("ConfirmAppointmentActivity", appointmentReservation);
            await context.CallActivityAsync("NotifySuccessActivity", appointmentReservation);
            return appointmentReservation;
        }

        // If we reach here, payment was not successful
        var isReleaseSuccessful = await context.CallActivityAsync<bool>("ReleaseAppointmentActivity", appointmentReservation);
        if (!isReleaseSuccessful)
        {
            logger.LogError($"Failed to release appointment slot for request. AppointmentId: {appointmentReservation.AppointmentId}");
        }

        var failureResult = new AppointmentResult
        {
            IsSuccessful = false,
            FailureReason = "Payment failed after 3 attempts.",
            AppointmentId = appointmentReservation.AppointmentId,
            CalendarId = appointmentReservation.CalendarId,
            PatientId = appointmentReservation.PatientId,
            AppointmentDate = appointmentReservation.AppointmentDate
        };

        await context.CallActivityAsync("NotifyFailureActivity", failureResult);

        return failureResult;
    }
}
