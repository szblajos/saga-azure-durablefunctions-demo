using MedicalBookingSystem.Models;
using MedicalBookingSystem.Services.Mocks;

namespace MedicalBookingSystem.Tests;

public class MockAppointmentServiceTests
{
    [Fact]
    public async Task ReserveAsync_ShouldCompleteWithoutError()
    {
        var service = new MockAppointmentService();
        await service.ReserveAsync(new AppointmentRequest
        {
            CalendarId = "calendar-123",
            PatientId = "patient-456",
            AppointmentDate = DateTime.UtcNow.AddDays(1)
        });
    }

    [Fact]
    public async Task ReserveAsync_ShouldReturnAppointmentId()
    {
        // Given
        var service = new MockAppointmentService();

        // When
        var result = await service.ReserveAsync(new AppointmentRequest
        {
            CalendarId = "calendar-123",
            PatientId = "patient-456",
            AppointmentDate = DateTime.UtcNow.AddDays(1)
        });

        // Then
        Assert.NotNull(result);
        Assert.NotNull(result.AppointmentId);
        Assert.NotEmpty(result.AppointmentId);
    }

    [Fact]
    public async Task ReserveAsync_ShouldReturnSuccess()
    {
        // Given
        var service = new MockAppointmentService();

        // When
        var result = await service.ReserveAsync(new AppointmentRequest
        {
            CalendarId = "calendar-123",
            PatientId = "patient-456",
            AppointmentDate = DateTime.UtcNow.AddDays(1)
        });

        // Then
        Assert.True(result.IsSuccessful);
    }


    [Fact]
    public async Task ReleaseAsync_ShouldCompleteWithoutError()
    {
        var service = new MockAppointmentService();
        await service.ReleaseAsync("apt-123");
    }

    [Fact]
    public async Task ConfirmAsync_ShouldCompleteWithoutError()
    {
        var service = new MockAppointmentService();
        await service.ConfirmAsync("apt-123");
    }
}
