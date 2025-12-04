# Contributing to Advanced C# Concepts

Thank you for your interest in contributing! This document provides guidelines and instructions for contributing to this project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [How to Contribute](#how-to-contribute)
- [Development Setup](#development-setup)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)
- [Pull Request Process](#pull-request-process)
- [Documentation](#documentation)

## Code of Conduct

This project adheres to the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code. Please report unacceptable behavior to dogaaydinn@gmail.com.

## Getting Started

### Prerequisites

- **.NET 8 SDK** (8.0.201 or later)
- **Git** for version control
- **IDE:** Visual Studio 2022, Rider, or VS Code with C# extension
- **Docker** (optional, for running observability stack)

### Fork and Clone

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```bash
   git clone https://github.com/YOUR_USERNAME/CSharp-Covariance-Polymorphism-Exercises.git
   cd CSharp-Covariance-Polymorphism-Exercises
   ```

3. Add upstream remote:
   ```bash
   git remote add upstream https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises.git
   ```

## How to Contribute

### Reporting Bugs

- **Search existing issues** before creating a new one
- Use the **bug report template**
- Include:
  - Clear description of the issue
  - Steps to reproduce
  - Expected vs actual behavior
  - Environment details (.NET version, OS)
  - Code samples or screenshots

### Suggesting Enhancements

- Use the **feature request template**
- Clearly describe the enhancement
- Explain why it would be valuable
- Provide examples if possible

### Code Contributions

We welcome contributions in these areas:

- **New Examples:** Additional C# concepts or patterns
- **Bug Fixes:** Fix issues in existing code
- **Performance Improvements:** Optimize existing examples
- **Documentation:** Improve guides, tutorials, or API docs
- **Tests:** Add missing test coverage
- **Benchmarks:** New performance benchmarks

## Development Setup

### Build the Project

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run specific test project
dotnet test tests/AdvancedConcepts.UnitTests
```

### Run the Application

```bash
# Run the core application
dotnet run --project src/AdvancedConcepts.Core

# Run with specific example
dotnet run --project src/AdvancedConcepts.Core -- --example polymorphism
```

### Run Benchmarks

```bash
# Run all benchmarks
dotnet run --project benchmarks/AdvancedConcepts.Benchmarks -c Release

# Run specific benchmark category
dotnet run --project benchmarks/AdvancedConcepts.Benchmarks -c Release -- --filter *Boxing*
```

### Docker Development Environment

```bash
# Start observability stack (Seq, Prometheus, Grafana)
docker-compose up -d

# View logs in Seq: http://localhost:5342
# View metrics in Grafana: http://localhost:3000
```

## Coding Standards

### C# Style Guidelines

We follow Microsoft's C# coding conventions with these tools enforcing standards:

- **StyleCop.Analyzers** - Code style rules
- **Roslynator.Analyzers** - Code quality rules
- **SonarAnalyzer.CSharp** - Security and code smell detection
- **Meziantou.Analyzer** - Best practices
- **SecurityCodeScan** - Security vulnerabilities

### Code Formatting

```bash
# Format code before committing
dotnet format

# Check formatting without changing files
dotnet format --verify-no-changes
```

### Naming Conventions

- **PascalCase:** Classes, methods, properties, events
- **camelCase:** Local variables, parameters
- **_camelCase:** Private fields
- **UPPER_CASE:** Constants

### File Organization

- One class per file
- File name matches class name
- Use file-scoped namespaces (C# 10+)
- Order members: fields, constructors, properties, methods

### Example Code Structure

```csharp
namespace AdvancedConcepts.Advanced;

public class ExamplePattern
{
    // Private fields
    private readonly IService _service;
    private readonly ILogger<ExamplePattern> _logger;

    // Constructor
    public ExamplePattern(IService service, ILogger<ExamplePattern> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Public properties
    public string Name { get; init; }

    // Public methods
    public async Task<Result<Data>> ProcessAsync(int id)
    {
        _logger.LogInformation("Processing {Id}", id);

        try
        {
            var data = await _service.GetDataAsync(id);
            return Result.Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process {Id}", id);
            return Result.Failure<Data>(new ProcessingError(ex.Message));
        }
    }

    // Private methods
    private void ValidateData(Data data)
    {
        // Validation logic
    }
}
```

## Testing Guidelines

### Unit Tests

- Use **xUnit** framework
- Use **FluentAssertions** for assertions
- Use **Moq** or **NSubstitute** for mocking
- Use **AutoFixture** for test data generation

```csharp
public class ExamplePatternTests
{
    private readonly Mock<IService> _serviceMock;
    private readonly ExamplePattern _sut;

    public ExamplePatternTests()
    {
        _serviceMock = new Mock<IService>();
        _sut = new ExamplePattern(_serviceMock.Object, Mock.Of<ILogger<ExamplePattern>>());
    }

    [Fact]
    public async Task ProcessAsync_WhenSuccessful_ReturnsSuccess()
    {
        // Arrange
        var expected = new Data { Id = 1, Name = "Test" };
        _serviceMock.Setup(s => s.GetDataAsync(1)).ReturnsAsync(expected);

        // Act
        var result = await _sut.ProcessAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }
}
```

### Test Coverage

- Aim for **>90% code coverage** for new code
- Use `dotnet test --collect:"XPlat Code Coverage"`
- View coverage reports in `coverage/` directory

### Property-Based Testing

- Use **FsCheck** for property-based tests
- Test invariants and properties
- Let FsCheck generate test cases

```csharp
[Property]
public Property ReverseTwice_ReturnsOriginal(int[] array)
{
    var result = array.Reverse().Reverse();
    return result.SequenceEqual(array).ToProperty();
}
```

## Pull Request Process

### Before Submitting

1. **Update from upstream:**
   ```bash
   git fetch upstream
   git rebase upstream/master
   ```

2. **Ensure all checks pass:**
   ```bash
   dotnet build
   dotnet test
   dotnet format --verify-no-changes
   ```

3. **Run security checks (if pre-commit installed):**
   ```bash
   pre-commit run --all-files
   ```

4. **Update documentation** if needed

### Creating Pull Request

1. **Create feature branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Commit with conventional commits:**
   ```bash
   git commit -m "feat: add covariance example for IEnumerable"
   git commit -m "fix: resolve null reference in boxing example"
   git commit -m "docs: update CONTRIBUTING.md with testing guidelines"
   ```

   **Commit types:** feat, fix, docs, style, refactor, perf, test, chore

3. **Push to your fork:**
   ```bash
   git push origin feature/your-feature-name
   ```

4. **Open Pull Request** on GitHub

### PR Requirements

- ✅ **Descriptive title** using conventional commits format
- ✅ **Description** explaining what and why
- ✅ **Link related issues** (Fixes #123)
- ✅ **All CI checks passing**
- ✅ **Tests added/updated**
- ✅ **Documentation updated**
- ✅ **Code reviewed** by at least one maintainer

### PR Review Process

1. **Automated checks** run (build, test, security)
2. **Code review** by maintainers
3. **Requested changes** addressed
4. **Approval** from maintainer
5. **Squash and merge** to main branch

## Documentation

### Code Documentation

- Add **XML documentation comments** for public APIs
- Include `<summary>`, `<param>`, `<returns>`, `<example>`
- Document complex algorithms or non-obvious logic

```csharp
/// <summary>
/// Processes the order asynchronously with retry logic.
/// </summary>
/// <param name="orderId">The unique identifier of the order to process.</param>
/// <returns>A result containing the processed order or an error.</returns>
/// <example>
/// <code>
/// var result = await ProcessOrderAsync(123);
/// if (result.IsSuccess)
/// {
///     Console.WriteLine($"Order processed: {result.Value.Id}");
/// }
/// </code>
/// </example>
public async Task<Result<Order>> ProcessOrderAsync(int orderId)
{
    // Implementation
}
```

### Markdown Documentation

- Use **clear headings** and structure
- Include **code examples** where helpful
- Add **diagrams** using Mermaid when appropriate
- Keep **line length** under 120 characters

## Recognition

Contributors will be recognized in:
- **Contributors section** of README
- **Release notes** for their contributions
- **GitHub contributors** page

## Questions?

- **Discussions:** Use [GitHub Discussions](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/discussions)
- **Issues:** Search existing issues or create new one
- **Email:** dogaaydinn@gmail.com for private inquiries

Thank you for contributing to Advanced C# Concepts! 🚀

---

**Last Updated:** 2025-11-30
**Version:** 1.0
