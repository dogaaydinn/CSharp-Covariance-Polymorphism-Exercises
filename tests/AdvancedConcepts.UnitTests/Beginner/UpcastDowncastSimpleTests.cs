using Xunit;
using FluentAssertions;
using AdvancedCsharpConcepts.Beginner.Upcast_Downcast;

namespace AdvancedConcepts.UnitTests.Beginner;

/// <summary>
/// Tests for basic upcasting and downcasting operations with Employee/Manager
/// </summary>
public class UpcastDowncastSimpleTests
{
    #region Upcasting Tests

    [Fact]
    public void Upcast_ManagerToEmployee_ShouldSucceed()
    {
        // Arrange
        var manager = new Manager();

        // Act
        Employee employee = manager;  // Implicit upcast

        // Assert
        employee.Should().NotBeNull();
        employee.Should().BeOfType<Manager>();
        employee.Should().BeSameAs(manager);
    }

    [Fact]
    public void Upcast_ManagerToEmployee_ShouldAccessBaseMethod()
    {
        // Arrange
        var manager = new Manager();

        // Act
        Employee employee = manager;

        // Assert - Should be able to call DisplayInfo
        Action act = () => employee.DisplayInfo();
        act.Should().NotThrow();
    }

    [Fact]
    public void Upcast_MultipleManagers_ToEmployeeArray()
    {
        // Arrange
        Manager[] managers = { new Manager(), new Manager(), new Manager() };

        // Act
        Employee[] employees = managers;

        // Assert
        employees.Should().HaveCount(3);
        employees.Should().AllBeOfType<Manager>();
    }

    #endregion

    #region Downcasting with 'as' Operator

    [Fact]
    public void Downcast_EmployeeToManager_WithManagerInstance_ShouldSucceed()
    {
        // Arrange
        Employee employee = new Manager();

        // Act
        Manager? manager = employee as Manager;

        // Assert
        manager.Should().NotBeNull();
        manager.Should().BeOfType<Manager>();
    }

    [Fact]
    public void Downcast_EmployeeToManager_WithEmployeeInstance_ShouldReturnNull()
    {
        // Arrange
        Employee employee = new Employee();

        // Act
        Manager? manager = employee as Manager;

        // Assert
        manager.Should().BeNull();
    }

    [Fact]
    public void Downcast_NullEmployee_ShouldReturnNull()
    {
        // Arrange
        Employee? employee = null;

        // Act
        Manager? manager = employee as Manager;

        // Assert
        manager.Should().BeNull();
    }

    #endregion

    #region Downcasting with Explicit Cast

    [Fact]
    public void ExplicitCast_EmployeeToManager_WithManagerInstance_ShouldSucceed()
    {
        // Arrange
        Employee employee = new Manager();

        // Act
        Manager manager = (Manager)employee;

        // Assert
        manager.Should().NotBeNull();
        manager.Should().BeOfType<Manager>();
    }

    [Fact]
    public void ExplicitCast_EmployeeToManager_WithEmployeeInstance_ShouldThrow()
    {
        // Arrange
        Employee employee = new Employee();

        // Act
        Action act = () => { Manager manager = (Manager)employee; };

        // Assert
        act.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void ExplicitCast_NullEmployee_ShouldThrow()
    {
        // Arrange
        Employee? employee = null;

        // Act
        Action act = () => { Manager manager = (Manager)employee!; };

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    #endregion

    #region 'is' Operator Tests

    [Fact]
    public void IsOperator_CheckIfEmployeeIsManager_WithManager_ShouldReturnTrue()
    {
        // Arrange
        Employee employee = new Manager();

        // Act
        bool isManager = employee is Manager;

        // Assert
        isManager.Should().BeTrue();
    }

    [Fact]
    public void IsOperator_CheckIfEmployeeIsManager_WithEmployee_ShouldReturnFalse()
    {
        // Arrange
        Employee employee = new Employee();

        // Act
        bool isManager = employee is Manager;

        // Assert
        isManager.Should().BeFalse();
    }

    [Fact]
    public void IsOperator_WithNull_ShouldReturnFalse()
    {
        // Arrange
        Employee? employee = null;

        // Act
        bool isManager = employee is Manager;

        // Assert
        isManager.Should().BeFalse();
    }

    [Fact]
    public void IsOperator_CheckBaseType_ShouldReturnTrue()
    {
        // Arrange
        Manager manager = new Manager();

        // Act
        bool isEmployee = manager is Employee;

        // Assert
        isEmployee.Should().BeTrue();
    }

    #endregion

    #region Pattern Matching Tests

    [Fact]
    public void PatternMatching_WithManager_ShouldMatchManagerType()
    {
        // Arrange
        Employee employee = new Manager();
        bool matchedManager = false;

        // Act
        if (employee is Manager manager)
        {
            matchedManager = true;
        }

        // Assert
        matchedManager.Should().BeTrue();
    }

    [Fact]
    public void PatternMatching_WithEmployee_ShouldNotMatchManager()
    {
        // Arrange
        Employee employee = new Employee();
        bool matchedManager = false;

        // Act
        if (employee is Manager manager)
        {
            matchedManager = true;
        }

        // Assert
        matchedManager.Should().BeFalse();
    }

    [Fact]
    public void SwitchPattern_WithDifferentTypes_ShouldMatchCorrectly()
    {
        // Arrange
        Employee[] employees = {
            new Employee(),
            new Manager(),
            new Employee(),
            new Manager()
        };

        // Act
        int managerCount = 0;
        int employeeCount = 0;

        foreach (var emp in employees)
        {
            switch (emp)
            {
                case Manager:
                    managerCount++;
                    break;
                case Employee:
                    employeeCount++;
                    break;
            }
        }

        // Assert
        managerCount.Should().Be(2);
        employeeCount.Should().Be(2);
    }

    [Fact]
    public void SwitchExpression_WithTypes_ShouldReturnCorrectString()
    {
        // Arrange
        Employee employee = new Manager();

        // Act
        string result = employee switch
        {
            Manager => "Manager",
            Employee => "Employee",
            _ => "Unknown"
        };

        // Assert
        result.Should().Be("Manager");
    }

    #endregion

    #region Type Testing and Conversion

    [Fact]
    public void GetType_ShouldReturnActualType()
    {
        // Arrange
        Employee employee = new Manager();

        // Act
        Type type = employee.GetType();

        // Assert
        type.Should().Be(typeof(Manager));
        type.Should().NotBe(typeof(Employee));
    }

    [Fact]
    public void GetType_WithEmployee_ShouldReturnEmployeeType()
    {
        // Arrange
        Employee employee = new Employee();

        // Act
        Type type = employee.GetType();

        // Assert
        type.Should().Be(typeof(Employee));
    }

    [Fact]
    public void IsAssignableFrom_EmployeeToManager_ShouldReturnTrue()
    {
        // Arrange & Act
        bool isAssignable = typeof(Employee).IsAssignableFrom(typeof(Manager));

        // Assert
        isAssignable.Should().BeTrue();
    }

    [Fact]
    public void IsAssignableFrom_ManagerToEmployee_ShouldReturnFalse()
    {
        // Arrange & Act
        bool isAssignable = typeof(Manager).IsAssignableFrom(typeof(Employee));

        // Assert
        isAssignable.Should().BeFalse();
    }

    #endregion

    #region Array Covariance

    [Fact]
    public void ArrayCovariance_ManagerArrayToEmployeeArray_ShouldSucceed()
    {
        // Arrange
        Manager[] managers = { new Manager(), new Manager() };

        // Act
        Employee[] employees = managers;

        // Assert
        employees.Should().HaveCount(2);
        employees.Should().AllBeOfType<Manager>();
    }

    [Fact]
    public void ArrayCovariance_WriteOperation_ShouldThrowAtRuntime()
    {
        // Arrange
        Manager[] managers = { new Manager() };
        Employee[] employees = managers;

        // Act
        Action act = () => employees[0] = new Employee();

        // Assert
        act.Should().Throw<ArrayTypeMismatchException>();
    }

    #endregion

    #region Real-World Scenarios

    [Fact]
    public void ProcessEmployees_WithMixedTypes_ShouldHandleBoth()
    {
        // Arrange
        Employee[] employees = {
            new Employee(),
            new Manager(),
            new Employee(),
            new Manager(),
            new Manager()
        };

        // Act
        var managers = employees.OfType<Manager>().ToList();
        var regularEmployees = employees.Where(e => e.GetType() == typeof(Employee)).ToList();

        // Assert
        managers.Should().HaveCount(3);
        regularEmployees.Should().HaveCount(2);
    }

    [Fact]
    public void FilterManagers_UsingLinq_ShouldReturnOnlyManagers()
    {
        // Arrange
        Employee[] employees = {
            new Employee(),
            new Manager(),
            new Manager()
        };

        // Act
        var managers = employees.OfType<Manager>().ToArray();

        // Assert
        managers.Should().HaveCount(2);
        managers.Should().AllBeOfType<Manager>();
    }

    #endregion
}
