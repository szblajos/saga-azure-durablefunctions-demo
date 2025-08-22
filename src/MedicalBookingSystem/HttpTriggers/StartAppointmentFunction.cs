using System.Net;
using MedicalBookingSystem.Models;
using MedicalBookingSystem.Orchestrators;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace MedicalBookingSystem.HttpTriggers;

public class StartAppointmentFunction
{
    [Function("StartAppointmentFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        var requestBody = await req.ReadFromJsonAsync<AppointmentRequest>();

        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(AppointmentOrchestrator), requestBody);

        var response = req.CreateResponse(HttpStatusCode.Accepted);
        await response.WriteStringAsync($"Orchestrator started. Instance ID: {instanceId}");

        return response;
    }
}
