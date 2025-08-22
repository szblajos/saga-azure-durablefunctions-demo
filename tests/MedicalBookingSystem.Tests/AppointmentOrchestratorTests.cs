using MedicalBookingSystem.Models;
using MedicalBookingSystem.Orchestrators;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedicalBookingSystem.Tests
{
    public class AppointmentOrchestratorTest
    {
        [Fact]
        public async Task Run_ReturnsFailure_WhenRequestIsNull()
        {
            // Arrange
            var contextMock = new Mock<TaskOrchestrationContext>();
            contextMock.Setup(c => c.GetInput<AppointmentRequest>()).Returns((AppointmentRequest?)null);
            contextMock.Setup(c => c.CreateReplaySafeLogger(It.IsAny<string>()))
                .Returns(Mock.Of<ILogger>());

            // Act
            var result = await AppointmentOrchestrator.Run(contextMock.Object);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Invalid appointment request.", result.FailureReason);
        }

        [Fact]
        public async Task Run_ReturnsFailure_WhenReservationFails()
        {
            // Arrange
            var request = new AppointmentRequest
            {
                CalendarId = "calendar-123",
                PatientId = "patient-456",
                AppointmentDate = DateTime.UtcNow.AddDays(1)
            };

            var failedResult = new AppointmentResult
            {
                IsSuccessful = false,
                CalendarId = request.CalendarId,
                PatientId = request.PatientId,
                AppointmentDate = request.AppointmentDate,
                FailureReason = "No slot"
            };

            var contextMock = new Mock<TaskOrchestrationContext>();
            contextMock.Setup(c => c.GetInput<AppointmentRequest>()).Returns(request);
            contextMock.Setup(c => c.CreateReplaySafeLogger(It.IsAny<string>()))
                .Returns(Mock.Of<ILogger>());
            contextMock.Setup(c => c.CallActivityAsync<AppointmentResult>("ReserveAppointmentActivity", request, It.IsAny<TaskOptions>()))
                .ReturnsAsync(failedResult);

            // Act
            var result = await AppointmentOrchestrator.Run(contextMock.Object);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("No slot", result.FailureReason);
        }

        [Fact]
        public async Task Run_ReturnsSuccess_WhenPaymentSucceedsFirstTry()
        {
            // Arrange
            var request = new AppointmentRequest
            {
                CalendarId = "calendar-123",
                PatientId = "patient-456",
                AppointmentDate = DateTime.UtcNow.AddDays(1)
            };

            var reservedResult = new AppointmentResult
            {
                IsSuccessful = true,
                AppointmentId = "A1",
                CalendarId = request.CalendarId,
                PatientId = request.PatientId,
                AppointmentDate = request.AppointmentDate
            };

            var contextMock = new Mock<TaskOrchestrationContext>();
            contextMock.Setup(c => c.GetInput<AppointmentRequest>()).Returns(request);
            contextMock.Setup(c => c.CreateReplaySafeLogger(It.IsAny<string>()))
                .Returns(Mock.Of<ILogger>());
            contextMock.Setup(c => c.CallActivityAsync<AppointmentResult>("ReserveAppointmentActivity", request, It.IsAny<TaskOptions>()))
                .ReturnsAsync(reservedResult);
            contextMock.Setup(c => c.CurrentUtcDateTime).Returns(DateTime.UtcNow);
            contextMock.SetupSequence(c => c.CallActivityAsync<bool>("TryPaymentActivity", reservedResult, It.IsAny<TaskOptions>()))
                .ReturnsAsync(true);

            // Act
            var result = await AppointmentOrchestrator.Run(contextMock.Object);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Equal("A1", result.AppointmentId);
        }

        [Fact]
        public async Task Run_ReturnsFailure_WhenPaymentFailsAllAttempts()
        {
            // Arrange
            var request = new AppointmentRequest
            {
                CalendarId = "calendar-123",
                PatientId = "patient-456",
                AppointmentDate = DateTime.UtcNow.AddDays(1)
            };
            
            var reservedResult = new AppointmentResult
            {
                IsSuccessful = true,
                AppointmentId = "A2",
                CalendarId = request.CalendarId,
                PatientId = request.PatientId,
                AppointmentDate = request.AppointmentDate
            };

            var contextMock = new Mock<TaskOrchestrationContext>();
            contextMock.Setup(c => c.GetInput<AppointmentRequest>()).Returns(request);
            contextMock.Setup(c => c.CreateReplaySafeLogger(It.IsAny<string>()))
                .Returns(Mock.Of<ILogger>());
            contextMock.Setup(c => c.CallActivityAsync<AppointmentResult>("ReserveAppointmentActivity", request, It.IsAny<TaskOptions>()))
                .ReturnsAsync(reservedResult);

            var now = DateTime.UtcNow;
            contextMock.SetupSequence(c => c.CurrentUtcDateTime)
                .Returns(now)
                .Returns(now.AddSeconds(10))
                .Returns(now.AddSeconds(20))
                .Returns(now.AddSeconds(30))
                .Returns(now.AddMinutes(11)); // Exceed deadline after 3 attempts

            contextMock.SetupSequence(c => c.CallActivityAsync<bool>("TryPaymentActivity", reservedResult, It.IsAny<TaskOptions>()))
                .ReturnsAsync(false)
                .ReturnsAsync(false)
                .ReturnsAsync(false);

            contextMock.Setup(c => c.CreateTimer(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            contextMock.Setup(c => c.CallActivityAsync<bool>("ReleaseAppointmentActivity", reservedResult, It.IsAny<TaskOptions>()))
                .ReturnsAsync(true);

            // Act
            var result = await AppointmentOrchestrator.Run(contextMock.Object);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal("Payment failed after 3 attempts.", result.FailureReason);
        }
    }
}