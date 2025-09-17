using Microsoft.Extensions.Logging;

namespace AzureFunctionCosmosTemplate.UnitTests;

/// <summary>
/// Base class for unit tests providing common test utilities and mocks.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Creates a mock logger for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to create a logger for.</typeparam>
    /// <returns>A mock logger instance.</returns>
    protected static Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    /// <summary>
    /// Verifies that a log message was written with the specified log level.
    /// </summary>
    /// <typeparam name="T">The type of the logger.</typeparam>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="logLevel">The expected log level.</param>
    /// <param name="times">The number of times the log should have been called.</param>
    protected static void VerifyLogWasCalled<T>(Mock<ILogger<T>> mockLogger, LogLevel logLevel, Times times)
    {
        mockLogger.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }

    /// <summary>
    /// Verifies that a log message was written with the specified log level and message content.
    /// </summary>
    /// <typeparam name="T">The type of the logger.</typeparam>
    /// <param name="mockLogger">The mock logger to verify.</param>
    /// <param name="logLevel">The expected log level.</param>
    /// <param name="expectedMessage">The expected message content.</param>
    /// <param name="times">The number of times the log should have been called.</param>
    protected static void VerifyLogWasCalled<T>(Mock<ILogger<T>> mockLogger, LogLevel logLevel, string expectedMessage, Times times)
    {
        mockLogger.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
}
