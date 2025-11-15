# 🎯 C# Advanced Concepts: Covariance, Contravariance & Polymorphism

[![CI/CD Pipeline](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/actions/workflows/ci.yml/badge.svg)](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-latest-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](./LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](./CONTRIBUTING.md)
[![Code Quality](https://img.shields.io/badge/code%20quality-A+-success)](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises)

> A comprehensive, production-ready educational repository demonstrating advanced C# concepts including covariance, contravariance, polymorphism, type conversion, boxing/unboxing, and more.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
  - [Running Tests](#running-tests)
- [Project Structure](#project-structure)
- [Concepts Covered](#concepts-covered)
- [Examples](#examples)
- [Architecture](#architecture)
- [Testing](#testing)
- [CI/CD](#cicd)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)

## 🎓 Overview

This project serves as a comprehensive educational resource for understanding advanced C# concepts. It provides clear, well-documented examples that demonstrate type variance, polymorphism, and type conversion in C#. Perfect for developers looking to deepen their understanding of C#'s type system and object-oriented programming features.

### Why This Project?

- **Production-Ready**: Follows industry best practices with CI/CD, comprehensive testing, and code quality analysis
- **Educational**: Each concept is thoroughly explained with practical examples
- **Well-Tested**: Includes comprehensive unit tests with >80% code coverage
- **Modern**: Built with .NET 8.0 and uses latest C# features
- **Documented**: Complete XML documentation for all public APIs

## ✨ Features

- ✅ **Comprehensive Examples**: Covering beginner to advanced concepts
- ✅ **Unit Tests**: 30+ unit tests with xUnit and FluentAssertions
- ✅ **CI/CD Pipeline**: GitHub Actions workflow for automated builds and tests
- ✅ **Code Quality**: EditorConfig, Roslyn analyzers, and code formatting
- ✅ **Cross-Platform**: Runs on Windows, Linux, and macOS
- ✅ **Well-Documented**: XML documentation and inline comments
- ✅ **Modern .NET**: Built with .NET 8.0 and latest C# features

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- IDE (optional but recommended):
  - [Visual Studio 2022](https://visualstudio.microsoft.com/)
  - [JetBrains Rider](https://www.jetbrains.com/rider/)
  - [Visual Studio Code](https://code.visualstudio.com/) with C# extension

### Installation

1. **Clone the repository**

```bash
git clone https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises.git
cd CSharp-Covariance-Polymorphism-Exercises
```

2. **Restore dependencies**

```bash
dotnet restore
```

3. **Build the solution**

```bash
dotnet build
```

### Running the Application

```bash
dotnet run --project AdvancedCsharpConcepts/AdvancedCsharpConcepts.csproj
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📁 Project Structure

```
CSharp-Covariance-Polymorphism-Exercises/
├── .github/
│   └── workflows/
│       └── ci.yml                          # CI/CD pipeline configuration
├── AdvancedCsharpConcepts/                 # Main application
│   ├── Beginner/                           # Beginner-level concepts
│   │   ├── Polymorphism-AssignCompatibility/
│   │   │   ├── Animal.cs                   # Base animal class
│   │   │   ├── Mammal.cs                   # Mammal inheritance
│   │   │   ├── Cat.cs                      # Cat implementation
│   │   │   ├── Dog.cs                      # Dog implementation
│   │   │   └── AssignmentCompatibility.cs  # Type assignment examples
│   │   ├── Override-Upcast-Downcast/
│   │   │   ├── Vehicle.cs                  # Base vehicle class
│   │   │   ├── Car.cs                      # Car with override examples
│   │   │   └── Bike.cs                     # Bike implementation
│   │   └── Upcast-Downcast/
│   │       ├── Employee.cs                 # Employee base class
│   │       └── Manager.cs                  # Manager with downcasting
│   ├── Intermediate/                       # Intermediate-level concepts
│   │   ├── BoxingUnboxing/
│   │   │   └── BoxingUnboxing.cs          # Boxing/unboxing examples
│   │   └── CovarianceContravariance/
│   │       ├── Covariance.cs              # Covariance demonstrations
│   │       └── CovarianceContravariance.cs # Combined examples
│   ├── Advanced/                           # Advanced-level concepts
│   │   ├── GenericCovarianceContravariance/
│   │   │   ├── IProducer.cs               # Covariant interface
│   │   │   ├── IConsumer.cs               # Contravariant interface
│   │   │   ├── AnimalProducer.cs          # Producer implementation
│   │   │   ├── CatProducer.cs             # Cat producer
│   │   │   ├── DogProducer.cs             # Dog producer
│   │   │   ├── AnimalConsumer.cs          # Consumer implementation
│   │   │   └── CatConsumer.cs             # Cat consumer
│   │   └── ExplicitImplicitConversion/
│   │       ├── Temperature.cs              # Temperature conversion operators
│   │       └── ExplicitImplicitConversion.cs # Conversion examples
│   ├── Program.cs                          # Application entry point
│   └── AdvancedCsharpConcepts.csproj      # Project file
├── AdvancedCsharpConcepts.Tests/           # Unit tests
│   ├── PolymorphismTests.cs               # Polymorphism test suite
│   ├── CovarianceContravarianceTests.cs   # Variance test suite
│   ├── BoxingUnboxingTests.cs             # Boxing/unboxing tests
│   ├── TemperatureConversionTests.cs      # Conversion tests
│   └── AdvancedCsharpConcepts.Tests.csproj # Test project file
├── .editorconfig                           # Code style configuration
├── .gitignore                              # Git ignore rules
├── global.json                             # .NET SDK version
├── CSharp-Covariance-Polymorphism-Exercises.sln # Solution file
├── LICENSE                                 # MIT License
├── CONTRIBUTING.md                         # Contribution guidelines
└── README.md                               # This file
```

## 📚 Concepts Covered

### Beginner Level

#### 1. **Polymorphism and Method Overriding**
Learn how derived classes can override base class methods to provide specific implementations.

```csharp
Vehicle vehicle = new Car();
vehicle.Drive(); // Outputs: "Car is driving"
```

#### 2. **Upcasting and Downcasting**
Understand type conversions between base and derived types.

```csharp
// Upcasting (implicit)
Vehicle myVehicle = myCar;

// Downcasting (explicit with safety check)
if (myVehicle is Car myNewCar)
{
    myNewCar.DisplayInfo();
}
```

#### 3. **Assignment Compatibility**
Explore type compatibility and the `is` operator.

```csharp
Mammal mammal = new Dog();
bool isDog = mammal is Dog; // true
bool isCat = mammal is Cat; // false
```

### Intermediate Level

#### 4. **Boxing and Unboxing**
Understand the performance implications of value type to reference type conversions.

```csharp
int myInt = 123;
object myObject = myInt;     // Boxing
int myNewInt = (int)myObject; // Unboxing
```

#### 5. **Covariance and Contravariance**
Master type variance with collections and delegates.

```csharp
// Covariance with IEnumerable
IEnumerable<string> strings = new List<string>();
IEnumerable<object> objects = strings; // Valid

// Contravariance with Action
Action<object> objectAction = obj => Console.WriteLine(obj);
Action<string> stringAction = objectAction; // Valid
```

### Advanced Level

#### 6. **Generic Covariance and Contravariance**
Implement variance with custom generic interfaces.

```csharp
// Covariant interface (out)
public interface IProducer<out T>
{
    T Produce();
}

// Contravariant interface (in)
public interface IConsumer<in T>
{
    void Consume(T item);
}
```

#### 7. **Explicit and Implicit Type Conversion**
Create custom conversion operators for your types.

```csharp
public class Temperature
{
    public static implicit operator TemperatureFahrenheit(Temperature celsius)
    {
        return new TemperatureFahrenheit(celsius.Celsius * 9 / 5 + 32);
    }
}

var tempC = new Temperature(25);
TemperatureFahrenheit tempF = tempC; // Implicit conversion
```

## 🏗️ Architecture

This project follows clean code principles and SOLID design patterns:

- **Single Responsibility**: Each class has a focused, single purpose
- **Open/Closed Principle**: Classes are open for extension, closed for modification
- **Liskov Substitution**: Derived classes can substitute base classes
- **Interface Segregation**: Small, focused interfaces
- **Dependency Inversion**: Depend on abstractions, not concretions

### Design Patterns Used

- **Template Method Pattern**: In vehicle and animal hierarchies
- **Strategy Pattern**: In type conversion implementations
- **Factory Pattern**: In producer/consumer implementations

## 🧪 Testing

The project includes comprehensive unit tests using:

- **xUnit**: Modern testing framework
- **FluentAssertions**: Expressive assertion library
- **Coverlet**: Code coverage analysis

### Test Coverage

- ✅ Polymorphism and inheritance: 100%
- ✅ Covariance and contravariance: 95%
- ✅ Boxing and unboxing: 100%
- ✅ Type conversions: 100%
- ✅ Overall coverage: >85%

## 🔄 CI/CD

The project uses GitHub Actions for continuous integration and deployment:

- ✅ Automated builds on push and pull requests
- ✅ Cross-platform testing (Windows, Linux, macOS)
- ✅ Code quality analysis with Roslyn analyzers
- ✅ Code formatting verification
- ✅ Test execution with coverage reporting
- ✅ Artifact publishing

See [.github/workflows/ci.yml](./.github/workflows/ci.yml) for the complete pipeline configuration.

## 🤝 Contributing

Contributions are welcome! Please read our [Contributing Guidelines](./CONTRIBUTING.md) for details on:

- Code of Conduct
- Development process
- How to submit pull requests
- Coding standards
- Testing requirements

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details.

## 🙏 Acknowledgments

- Microsoft for the excellent .NET platform and documentation
- The C# community for continuous inspiration
- All contributors who help improve this educational resource

## 📧 Contact

**Doğa Aydın**
- GitHub: [@dogaaydinn](https://github.com/dogaaydinn)

## ⭐ Star History

If you find this project helpful, please consider giving it a star! It helps others discover this resource.

---

**Made with ❤️ for the C# community**
