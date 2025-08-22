namespace MedicalBookingSystem.Services.Mocks;

public class MockPaymentService : IPaymentService
{
    private readonly Random _random = new();

    public Task<bool> TryPaymentAsync(string appointmentId, string patientId)
    {
        var success = _random.Next(0, 2) == 1;
        Console.WriteLine($"[MockPaymentService] Try payment ({appointmentId}, {patientId}): {(success ? "Successful" : "Unsuccessful")}");
        return Task.FromResult(success);
    }
}