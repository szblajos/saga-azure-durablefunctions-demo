using MedicalBookingSystem.Models;
using MedicalBookingSystem.Services.Mocks;

namespace MedicalBookingSystem.Tests;

public class MockNotificationServiceTests
{
    [Fact]
    public async Task SendSuccessNotificationAsync_ShouldCompleteWithoutError()
    {
        var service = new MockNotificationService();
        await service.SendSuccessNotificationAsync(new AppointmentResult
        {
            AppointmentId = "apt-123",
            PatientId = "patient-456",
            CalendarId = "calendar-789",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            IsSuccessful = true
        });
    }

    [Fact]
    public async Task SendFailureNotificationAsync_ShouldCompleteWithoutError()
    {
        var service = new MockNotificationService();
        await service.SendFailureNotificationAsync(new FailureNotification
        {
            AppointmentId = "apt-123",
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Reason = "Some failure reason"
        });

    }
}
