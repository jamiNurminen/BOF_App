# Backend Test Suite

This test suite provides comprehensive coverage for the BOF App backend API, including:

- **BanknotesController Tests**: Validation of date ranges, error handling, and data retrieval
- **ExchangeRatesController Tests**: Currency conversion, input validation, and error scenarios
- **Service Layer Tests**: Business logic validation and calculations

## Running the Tests

### Prerequisites
- .NET 10.0 SDK installed
- The backend project builds successfully

### Run All Tests
```bash
cd /Users/jaminurminen/Projects/BOF_App
dotnet test
```

### Run Tests with Verbose Output
```bash
dotnet test --verbosity detailed
```

### Run Specific Test Class
```bash
dotnet test --filter "NamespaceName.ClassName"
```

### Run Tests with Coverage Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## Test Structure

### BanknotesControllerTests
- ✅ Valid date ranges return OK with data
- ✅ Missing start or end date returns BadRequest
- ✅ Start date after end date returns BadRequest  
- ✅ Future end dates return BadRequest
- ✅ Breakdown data is correctly included

### ExchangeRatesControllerTests
- ✅ Valid currencies and amount return OK with conversions
- ✅ Zero or negative amounts return BadRequest
- ✅ Null or empty currencies return BadRequest
- ✅ KeyNotFoundException returns NotFound
- ✅ Generic exceptions return 500 Internal Server Error
- ✅ Multiple currencies are properly handled
- ✅ Available currencies endpoint returns full list

### ExchangeRatesServiceTests
- ✅ Exchange rate parsing with various decimal formats (period and comma)
- ✅ Invalid amounts are rejected
- ✅ Currency conversion calculations are accurate
- ✅ Real-world exchange rate scenarios

### BanknotesServiceTests
- ✅ Correct date formatting for API calls (M/d/yyyy format)
- ✅ Denomination parsing logic

## Test Dependencies

- **xUnit**: Testing framework
- **Moq**: Mocking library
- **FluentAssertions**: Assertions library for readable test code
- **Microsoft.AspNetCore.Mvc.Testing**: ASP.NET Core testing utilities

## Adding New Tests

When adding new endpoints or functionality:

1. Create test methods in the appropriate test class
2. Follow the Arrange-Act-Assert pattern
3. Use descriptive test names: `MethodName_Scenario_ExpectedResult`
4. Mock external dependencies (HttpClient, databases, etc.)
5. Use FluentAssertions for readable validation

Example:
```csharp
[Fact]
public async Task GetBanknotes_WithValidDates_ReturnsOkWithData()
{
    // Arrange
    var startDate = new DateTime(2025, 1, 1);
    var endDate = new DateTime(2025, 12, 31);
    
    // Act
    var result = await _controller.GetBanknotes(startDate, endDate);
    
    // Assert
    result.Should().BeOfType<OkObjectResult>();
}
```

## CI/CD Integration

These tests can be integrated into your CI/CD pipeline (GitHub Actions, Azure Pipelines, etc.):

```bash
dotnet test --logger "trx;LogFileName=test-results.trx"
```

This generates an xUnit test result file that most CI/CD systems can parse.
