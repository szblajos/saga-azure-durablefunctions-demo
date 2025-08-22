# Saga Pattern Demo Using Azure Durable Functions

## Concept

This project demonstrates the Saga pattern for distributed transactions using Azure Durable Functions. The workflow simulates a medical appointment booking process, where each step (reservation, payment, confirmation, notification) is handled as a separate activity. If any step fails (e.g., payment), the orchestrator triggers compensating actions (e.g., releasing the reservation and sending a failure notification).

## Prerequisites

- **Azurite VS Code Extension** (for local Azure Storage emulation)
- **Azure Functions VS Code Extension**
- **Azure Functions Core Tools** (`func`)
- **Azure SDK for .NET**
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- (Optional) [Postman](https://www.postman.com/) or `curl` for testing HTTP endpoints

## Directory Structure

```txt
saga-azure-durablefunctions-demo/
├── README.md
├── saga-azure-durablefunctions-demo.sln
├── src/
│   └── MedicalBookingSystem/
│       ├── Activities/
│       │   ├── ConfirmAppointmentActivity.cs
│       │   ├── NotifyFailureActivity.cs
│       │   ├── NotifySuccessActivity.cs
│       │   ├── ReleaseAppointmentActivity.cs
│       │   ├── ReserveAppointmentActivity.cs
│       │   └── TryPaymentActivity.cs
│       ├── HttpTriggers/
│       │   └── StartAppointmentFunction.cs
│       ├── Models/
│       │   ├── AppointmentRequest.cs
│       │   ├── AppointmentResult.cs
│       │   └── FailureNotification.cs
│       ├── Orchestrators/
│       │   └── AppointmentOrchestrator.cs
│       ├── Services/
│       │   ├── IAppointmentService.cs
│       │   ├── INofiticationService.cs
│       │   ├── IPaymentService.cs
│       │   └── Mocks/
│       ├── Program.cs
│       ├── host.json
│       └── local.settings.json
└── tests/
    └── MedicalBookingSystem.Tests/
        └── (unit tests)
```

## Workflow

1. **StartAppointmentFunction** (HTTP Trigger) receives a booking request and starts the `AppointmentOrchestrator`.
2. **AppointmentOrchestrator** coordinates the following activities:
   - `ReserveAppointmentActivity`: Reserves the appointment slot.
   - `TryPaymentActivity`: Attempts payment up to 3 times.
   - If payment succeeds:
     - `ConfirmAppointmentActivity`: Confirms the reservation.
     - `NotifySuccessActivity`: Notifies the user of success.
   - If payment fails after 3 attempts:
     - `ReleaseAppointmentActivity`: Releases the reserved slot.
     - `NotifyFailureActivity`: Notifies the user of failure.

## Running the Demo Project

### 1. Start Azurite

- Open the Command Palette in VS Code: `Ctrl` + `Shift` + `P`
- Select: `Azurite: Start`

### 2. Run the Function App

- Open a terminal in the project root.
- Run:

  ``` sh
  func start
  ```

### 3. Send a Test Request

You can use `curl` to trigger the workflow. Example:

```sh
curl -X POST http://localhost:7071/api/StartAppointmentFunction \
  -H "Content-Type: application/json" \
  -d "{\"CalendarId\":\"calendar-1\",\"PatientId\":\"patient-42\",\"AppointmentDate\":\"2025-09-01T10:00:00Z\"}"
```

### 4. Watch the Logs

- Observe the function logs in the terminal for orchestration progress and activity results.

## Stopping and Cleaning Up

- **Stop the function app:** Press `Ctrl` + `C` in the terminal.
- **Stop Azurite:** Open the Command Palette and select `Azurite: Close`.
- **Clean up Azurite:** Open the Command Palette and select `Azurite: Clean`.

## Conclusion

This demo showcases how to implement the Saga pattern for distributed transactions using Azure Durable Functions. By breaking down a business process into orchestrated activities with compensation logic, you can build robust, reliable workflows that handle failures gracefully. The project provides a practical foundation for designing resilient microservices and serverless solutions in the Azure ecosystem. Feel free to extend or adapt the workflow to fit your own business scenarios!

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing & Issues

Feel free to open issues or feature requests on GitHub if you encounter any problems or have suggestions for improvement.

GitHub repository: [szblajos/saga-azure-durablefunctions-demo](https://github.com/szblajos/saga-azure-durablefunctions-demo)
