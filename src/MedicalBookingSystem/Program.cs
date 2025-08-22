using MedicalBookingSystem.Services;
using MedicalBookingSystem.Services.Mocks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// Add services to DI
builder.Services.AddScoped<IPaymentService, MockPaymentService>();
builder.Services.AddScoped<IAppointmentService, MockAppointmentService>();
builder.Services.AddScoped<INotificationService, MockNotificationService>();

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
