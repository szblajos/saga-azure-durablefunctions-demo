using MedicalBookingSystem.Services.Mocks;

namespace MedicalBookingSystem.Tests;

public class MockPaymentServiceTests
{
    [Fact]
    public async Task TryPaymentAsync_ShouldReturnBoolean()
    {
        var service = new MockPaymentService();
        var result = await service.TryPaymentAsync("apt-123", "patient-456");

        Assert.IsType<bool>(result);
    }
}
