# Tests

This directory contains all test projects for the Azure Function Cosmos Template solution.

## 🧪 Test Projects

### AzureFunctionCosmosTemplate.UnitTests

Unit tests for all layers of the application, organized by project structure:

```
Tests/AzureFunctionCosmosTemplate.UnitTests/
├── Application/
│   └── Services/           # Tests for business logic services
├── Domain/
│   ├── DTOs/              # Tests for data transfer objects
│   └── Extensions/        # Tests for mapping extension methods
├── Function/
│   └── Extensions/        # Tests for HTTP response extensions
├── Repository/            # Tests for data access layer (when added)
├── TestBase.cs            # Base class with common test utilities
└── GlobalUsings.cs        # Global using statements
```

## 🛠️ Test Technologies

- **xUnit**: Testing framework
- **FluentAssertions**: Assertion library for readable tests
- **Moq**: Mocking framework for dependencies
- **Coverlet**: Code coverage analysis

## 🚀 Running Tests

### Command Line

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests in a specific project
dotnet test Tests/AzureFunctionCosmosTemplate.UnitTests

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests and generate coverage report
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

### Visual Studio / VS Code

- Use the built-in test explorer
- Right-click on test methods/classes to run specific tests
- Use debugging capabilities to step through test code

## 📊 Test Coverage

The tests aim to provide comprehensive coverage for:

- ✅ **Domain Layer**: DTOs, entities, and mapping logic
- ✅ **Application Layer**: Business logic and service operations
- ✅ **Function Layer**: HTTP response extensions and utilities
- 🔄 **Repository Layer**: Data access operations (to be added)

## 📝 Test Patterns

### Arrange-Act-Assert (AAA)

All tests follow the AAA pattern:

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var input = "test data";
    var expected = "expected result";

    // Act
    var result = await _service.MethodName(input);

    // Assert
    result.Should().Be(expected);
}
```

### Test Naming Convention

Tests are named using the pattern: `MethodName_Scenario_ExpectedResult`

Examples:
- `CreateUserAsync_WithValidDto_ShouldCreateAndReturnUserDto`
- `GetUserByIdAsync_WithNonExistentId_ShouldReturnNull`
- `UpdateUserAsync_WithExistingEmail_ShouldThrowInvalidOperationException`

### Mocking Strategy

- Mock all external dependencies (repositories, loggers, etc.)
- Use `Mock<T>` from Moq framework
- Verify important method calls and interactions
- Use `TestBase` class for common mock creation utilities

## 🎯 Test Categories

### Unit Tests

- Test individual methods and classes in isolation
- Mock all external dependencies
- Fast execution (< 100ms per test)
- High code coverage target (> 80%)

### Integration Tests (Future)

- Test interactions between components
- Use test databases or in-memory implementations
- Validate end-to-end workflows

### Performance Tests (Future)

- Validate response times and resource usage
- Load testing for HTTP endpoints
- Database query performance validation

## 🔧 Adding New Tests

When adding new functionality:

1. **Create corresponding test files** in the appropriate directory structure
2. **Follow naming conventions** for test classes and methods
3. **Use TestBase utilities** for common operations
4. **Mock external dependencies** appropriately
5. **Test both success and error scenarios**
6. **Verify logging behavior** when applicable

### Example Test Structure

```csharp
public class NewServiceTests : TestBase
{
    private readonly Mock<IDependency> _mockDependency;
    private readonly Mock<ILogger<NewService>> _mockLogger;
    private readonly NewService _service;

    public NewServiceTests()
    {
        _mockDependency = new Mock<IDependency>();
        _mockLogger = CreateMockLogger<NewService>();
        _service = new NewService(_mockDependency.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Method_Scenario_ExpectedResult()
    {
        // Test implementation
    }
}
```

## 📈 Continuous Integration

Tests are designed to run in CI/CD pipelines:

- **Fast execution**: All unit tests should complete in under 30 seconds
- **Deterministic**: Tests should produce consistent results
- **Independent**: Tests don't depend on external services
- **Clean**: Tests clean up after themselves

## 🐛 Debugging Tests

Tips for debugging failing tests:

1. **Use descriptive test names** to quickly identify what's being tested
2. **Check assertion messages** from FluentAssertions for clear error descriptions
3. **Use debugger** to step through test execution
4. **Verify mock setups** are correct for the scenario being tested
5. **Check test data** matches expectations

## 📚 Resources

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/)
