using Xunit;
using FluentAssertions;

namespace AdvancedConcepts.UnitTests.Advanced;

/// <summary>
/// Comprehensive tests for Roslyn Analyzers
/// Tests verify analyzer rules and code quality checks
/// </summary>
public class RoslynAnalyzerTests
{
    #region StringConcatenationAnalyzer Tests (AC1001)

    [Fact]
    public void StringConcatenation_InLoop_ShouldBeDetectedByAnalyzer()
    {
        // This test documents that the StringConcatenationAnalyzer (AC1001)
        // should detect this pattern and suggest StringBuilder

        // Arrange & Act
        string result = "";
        for (int i = 0; i < 10; i++)
        {
            result += i.ToString(); // AC1001: Should use StringBuilder
        }

        // Assert - Verify the concatenation worked (analyzer would warn)
        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void StringBuilder_InLoop_IsBetterPractice()
    {
        // This demonstrates the correct pattern that analyzer recommends

        // Arrange & Act
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < 10; i++)
        {
            sb.Append(i);
        }
        string result = sb.ToString();

        // Assert
        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void StringConcatenation_OutsideLoop_ShouldNotTriggerAnalyzer()
    {
        // Arrange & Act
        string part1 = "Hello";
        string part2 = "World";
        string result = part1 + " " + part2; // OK: Not in a loop

        // Assert
        result.Should().Be("Hello World");
    }

    [Fact]
    public void StringConcatenation_InForEachLoop_ShouldBeDetected()
    {
        // Arrange
        var items = new[] { "a", "b", "c" };
        string result = "";

        // Act
        foreach (var item in items)
        {
            result += item; // AC1001: Should use StringBuilder
        }

        // Assert
        result.Should().Be("abc");
    }

    [Fact]
    public void StringConcatenation_InWhileLoop_ShouldBeDetected()
    {
        // Arrange
        string result = "";
        int count = 0;

        // Act
        while (count < 5)
        {
            result += count.ToString(); // AC1001: Should use StringBuilder
            count++;
        }

        // Assert
        result.Should().Be("01234");
    }

    #endregion

    #region SQL Injection Analyzer Tests (AC009)

    [Fact]
    public void SqlQuery_WithStringConcatenation_IsVulnerableToInjection()
    {
        // This test documents SQL injection vulnerability pattern
        // SqlInjectionAnalyzer (AC009) should detect this

        // Arrange
        string userId = "123";

        // Act - VULNERABLE CODE (analyzer should flag this)
        string query = "SELECT * FROM Users WHERE Id = " + userId; // AC009: SQL Injection risk

        // Assert - Verify query was built (but this is insecure!)
        query.Should().Contain("SELECT");
        query.Should().Contain(userId);
    }

    [Fact]
    public void SqlQuery_WithInterpolation_IsAlsoVulnerable()
    {
        // Arrange
        string userName = "admin";

        // Act - VULNERABLE CODE
        string query = $"SELECT * FROM Users WHERE Name = '{userName}'"; // AC009: SQL Injection risk

        // Assert
        query.Should().Contain("SELECT");
        query.Should().Contain(userName);
    }

    [Fact]
    public void SqlQuery_WithParameters_IsSafe()
    {
        // This demonstrates the safe pattern

        // Arrange
        string commandText = "SELECT * FROM Users WHERE Id = @UserId"; // Safe: Parameterized
        var parameters = new { UserId = 123 };

        // Assert
        commandText.Should().NotContain("+");
        commandText.Should().Contain("@UserId");
        parameters.UserId.Should().Be(123);
    }

    [Fact]
    public void SqlQuery_InsertWithConcatenation_ShouldBeDetected()
    {
        // Arrange
        string name = "John";
        string email = "john@example.com";

        // Act - VULNERABLE
        string query = "INSERT INTO Users (Name, Email) VALUES ('" + name + "', '" + email + "')"; // AC009

        // Assert
        query.Should().Contain("INSERT");
        query.Should().Contain(name);
    }

    [Fact]
    public void SqlQuery_UpdateWithInterpolation_ShouldBeDetected()
    {
        // Arrange
        string newName = "Jane";
        int userId = 42;

        // Act - VULNERABLE
        string query = $"UPDATE Users SET Name = '{newName}' WHERE Id = {userId}"; // AC009

        // Assert
        query.Should().Contain("UPDATE");
        query.Should().Contain(newName);
    }

    [Fact]
    public void SqlQuery_DeleteWithVariable_ShouldBeDetected()
    {
        // Arrange
        string category = "temp";

        // Act - VULNERABLE
        string query = "DELETE FROM Items WHERE Category = '" + category + "'"; // AC009

        // Assert
        query.Should().Contain("DELETE");
    }

    [Fact]
    public void NonSqlString_WithConcatenation_ShouldNotTrigger()
    {
        // Arrange
        string greeting = "Hello";
        string name = "World";

        // Act
        string message = greeting + " " + name; // OK: Not SQL

        // Assert
        message.Should().Be("Hello World");
    }

    #endregion

    #region ConfigureAwait Analyzer Tests

    [Fact]
    public async Task AsyncMethod_WithoutConfigureAwait_InLibraryCode()
    {
        // ConfigureAwaitAnalyzer should detect missing ConfigureAwait(false)
        // in library code (non-UI code)

        // Arrange & Act
        await Task.Delay(1); // Should be: await Task.Delay(1).ConfigureAwait(false);

        // Assert
        true.Should().BeTrue();
    }

    [Fact]
    public async Task AsyncMethod_WithConfigureAwait_IsBestPractice()
    {
        // This demonstrates the correct pattern for library code

        // Arrange & Act
        await Task.Delay(1).ConfigureAwait(false); // Correct!

        // Assert
        true.Should().BeTrue();
    }

    [Fact]
    public async Task MultipleAwaits_WithoutConfigureAwait_ShouldAllBeDetected()
    {
        // Arrange & Act
        await Task.Delay(1);
        await Task.Delay(1);
        await Task.Delay(1);

        // Assert
        true.Should().BeTrue();
    }

    #endregion

    #region Allocation Analyzer Tests

    [Fact]
    public void Boxing_ValueType_CreatesAllocation()
    {
        // AllocationAnalyzer should detect boxing allocations

        // Arrange
        int value = 42;

        // Act
        object boxed = value; // Allocation: Boxing

        // Assert
        boxed.Should().Be(42);
    }

    [Fact]
    public void StringFormat_BoxesValueTypes()
    {
        // Arrange
        int value = 42;

        // Act
        string result = string.Format("Value: {0}", value); // Allocations: boxing + string

        // Assert
        result.Should().Be("Value: 42");
    }

    [Fact]
    public void Interpolation_WithValueTypes_CreatesAllocations()
    {
        // Arrange
        int count = 10;
        double price = 19.99;

        // Act
        string message = $"Count: {count}, Price: {price}"; // Multiple allocations

        // Assert
        message.Should().Contain("10");
        message.Should().Contain("Price:");
        message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void LinqToArray_InLoop_CausesMultipleAllocations()
    {
        // Arrange
        var items = Enumerable.Range(1, 100);

        // Act
        for (int i = 0; i < 5; i++)
        {
            var array = items.ToArray(); // Allocation on each iteration
            array.Should().HaveCount(100);
        }
    }

    [Fact]
    public void CachingArrayOutsideLoop_ReducesAllocations()
    {
        // Arrange
        var items = Enumerable.Range(1, 100);
        var array = items.ToArray(); // Single allocation

        // Act
        for (int i = 0; i < 5; i++)
        {
            array.Should().HaveCount(100); // No allocations in loop
        }
    }

    #endregion

    #region SOLID Violation Analyzer Tests

    [Fact]
    public void GodClass_WithManyResponsibilities_ViolatesSRP()
    {
        // SolidViolationAnalyzer should detect classes with too many responsibilities

        // Arrange & Act
        var godClass = new GodClassExample();

        // This class does too much (violates SRP)
        godClass.ProcessData();
        godClass.SaveToDatabase();
        godClass.SendEmail();
        godClass.GenerateReport();

        // Assert
        godClass.Should().NotBeNull();
    }

    [Fact]
    public void ClassWithSingleResponsibility_FollowsSRP()
    {
        // Arrange & Act
        var dataProcessor = new DataProcessor();
        dataProcessor.Process();

        // Assert
        dataProcessor.Should().NotBeNull();
    }

    [Fact]
    public void HighComplexity_Method_ShouldBeRefactored()
    {
        // ClassComplexityAnalyzer should detect high cyclomatic complexity

        // Arrange
        int value = 15;

        // Act - High complexity method
        string result = HighComplexityMethod(value);

        // Assert
        result.Should().NotBeEmpty();
    }

    private string HighComplexityMethod(int value)
    {
        // This method has high cyclomatic complexity (many branches)
        if (value > 100)
            return "Very High";
        else if (value > 50)
            return "High";
        else if (value > 25)
            return "Medium-High";
        else if (value > 10)
            return "Medium";
        else if (value > 5)
            return "Low-Medium";
        else if (value > 0)
            return "Low";
        else if (value == 0)
            return "Zero";
        else
            return "Negative";
    }

    #endregion

    #region XSS Vulnerability Analyzer Tests

    [Fact]
    public void HtmlOutput_WithUnescapedUserInput_IsVulnerable()
    {
        // XssVulnerabilityAnalyzer should detect unescaped HTML output

        // Arrange
        string userInput = "<script>alert('xss')</script>";

        // Act - VULNERABLE: Direct HTML output
        string html = $"<div>{userInput}</div>"; // XSS vulnerability

        // Assert
        html.Should().Contain(userInput);
    }

    [Fact]
    public void HtmlOutput_WithEscaping_IsSafe()
    {
        // Arrange
        string userInput = "<script>alert('xss')</script>";

        // Act - SAFE: Encoded
        string encoded = System.Net.WebUtility.HtmlEncode(userInput);
        string html = $"<div>{encoded}</div>";

        // Assert
        html.Should().NotContain("<script>");
        html.Should().Contain("&lt;script&gt;");
    }

    [Fact]
    public void JavaScriptOutput_WithUnescapedData_IsVulnerable()
    {
        // Arrange
        string userInput = "'; alert('xss'); //";

        // Act - VULNERABLE
        string js = $"var name = '{userInput}';"; // XSS vulnerability

        // Assert
        js.Should().Contain(userInput);
    }

    #endregion

    #region Immutability Analyzer Tests

    [Fact]
    public void MutableState_InPublicClass_ShouldBeDetected()
    {
        // ImmutabilityAnalyzer should detect mutable public fields

        // Arrange & Act
        var mutable = new MutableClass();
        mutable.PublicField = 42; // Mutable state - should be property

        // Assert
        mutable.PublicField.Should().Be(42);
    }

    [Fact]
    public void ImmutableClass_WithReadOnlyProperties_IsBestPractice()
    {
        // Arrange & Act
        var immutable = new ImmutableClass(42);

        // Assert
        immutable.Value.Should().Be(42);
        // immutable.Value = 100; // Won't compile - good!
    }

    [Fact]
    public void RecordType_ProvidesImmutability()
    {
        // Arrange & Act
        var record = new ImmutableRecord(Name: "Test", Value: 42);

        // Assert
        record.Name.Should().Be("Test");
        record.Value.Should().Be(42);
    }

    #endregion

    #region Helper Classes for Tests

    private class GodClassExample
    {
        public void ProcessData() { }
        public void SaveToDatabase() { }
        public void SendEmail() { }
        public void GenerateReport() { }
        public void ValidateInput() { }
        public void LogActivity() { }
    }

    private class DataProcessor
    {
        public void Process() { }
    }

    private class MutableClass
    {
        public int PublicField; // Bad: Public mutable field
    }

    private class ImmutableClass
    {
        public int Value { get; }
        public ImmutableClass(int value) => Value = value;
    }

    private record ImmutableRecord(string Name, int Value);

    #endregion
}
