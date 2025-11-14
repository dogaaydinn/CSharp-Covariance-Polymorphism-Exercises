# 🏗️ Architecture Overview

## System Architecture

This document describes the enterprise-level architecture of the Advanced C# Concepts project.

---

## 📐 High-Level Architecture (C4 Model)

### Level 1: System Context Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│                    Advanced C# Concepts                         │
│                  Educational Framework                          │
│                                                                 │
│  Purpose: Demonstrate advanced C# programming patterns         │
│  Technology: .NET 8, C# 12                                     │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
                            ▲
                            │
                            │
                    ┌───────┴────────┐
                    │                │
               ┌────▼────┐      ┌───▼────┐
               │         │      │        │
               │ Developers    │  Students │
               │         │      │        │
               └─────────┘      └────────┘
```

### Level 2: Container Diagram

```
┌──────────────────────────────────────────────────────────────────┐
│                  Advanced C# Concepts System                     │
│                                                                  │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────────┐  │
│  │                │  │                │  │                  │  │
│  │  Core Library  │  │   Demo CLI     │  │   Benchmarks     │  │
│  │                │  │                │  │                  │  │
│  │ - Polymorphism │  │ - Examples     │  │ - BenchmarkDotNet│  │
│  │ - Generics     │  │ - Tutorials    │  │ - Profiling      │  │
│  │ - Conversions  │  │ - Interactive  │  │ - Comparisons    │  │
│  │                │  │                │  │                  │  │
│  └────────┬───────┘  └───────┬────────┘  └────────┬─────────┘  │
│           │                  │                     │            │
│           └──────────────────┼─────────────────────┘            │
│                              │                                  │
│                     ┌────────▼─────────┐                        │
│                     │                  │                        │
│                     │  Testing Layer   │                        │
│                     │                  │                        │
│                     │  - Unit Tests    │                        │
│                     │  - Integration   │                        │
│                     │  - Mutation      │                        │
│                     │                  │                        │
│                     └──────────────────┘                        │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

### Level 3: Component Diagram

```
Core Library Components:

┌─────────────────────────────────────────────────────────────┐
│                     Core Library                            │
│                                                             │
│  ┌──────────────────┐    ┌──────────────────┐             │
│  │  Polymorphism    │    │  Type Variance   │             │
│  │  Components      │    │  Components      │             │
│  │                  │    │                  │             │
│  │ - Vehicle        │    │ - IProducer<T>   │             │
│  │ - Car/Bike       │    │ - IConsumer<T>   │             │
│  │ - Animal/Mammal  │    │ - Covariance     │             │
│  │ - Cat/Dog        │    │ - Contravariance │             │
│  └──────────────────┘    └──────────────────┘             │
│                                                             │
│  ┌──────────────────┐    ┌──────────────────┐             │
│  │  Type Conversion │    │  Memory Mgmt     │             │
│  │  Components      │    │  Components      │             │
│  │                  │    │                  │             │
│  │ - Temperature    │    │ - Boxing         │             │
│  │ - Implicit Ops   │    │ - Unboxing       │             │
│  │ - Explicit Ops   │    │ - Value Types    │             │
│  │ - Pattern Match  │    │ - Ref Types      │             │
│  └──────────────────┘    └──────────────────┘             │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 🏛️ Architectural Patterns

### 1. Layered Architecture

```
┌─────────────────────────────────────────────┐
│         Presentation Layer                  │
│  (Console Application / Demos)              │
└──────────────┬──────────────────────────────┘
               │
┌──────────────▼──────────────────────────────┐
│         Application Layer                   │
│  (Use Cases / Business Logic)               │
└──────────────┬──────────────────────────────┘
               │
┌──────────────▼──────────────────────────────┐
│         Domain Layer                        │
│  (Core Concepts / Entities)                 │
└──────────────┬──────────────────────────────┘
               │
┌──────────────▼──────────────────────────────┐
│         Infrastructure Layer                │
│  (Logging / Metrics / I/O)                  │
└─────────────────────────────────────────────┘
```

### 2. SOLID Principles Implementation

#### Single Responsibility Principle (SRP)
Each class has one reason to change:
- `Vehicle` - Handles vehicle behavior
- `Temperature` - Handles temperature conversions
- `BoxingUnboxing` - Demonstrates boxing/unboxing

#### Open/Closed Principle (OCP)
Open for extension, closed for modification:
```csharp
public abstract class Vehicle  // Closed for modification
{
    public abstract void Drive();
}

public class ElectricCar : Vehicle  // Open for extension
{
    public override void Drive() => Console.WriteLine("Electric drive");
}
```

#### Liskov Substitution Principle (LSP)
Subtypes must be substitutable for base types:
```csharp
Vehicle vehicle = new Car(); // LSP - Car can substitute Vehicle
vehicle.Drive(); // Works correctly
```

#### Interface Segregation Principle (ISP)
Many specific interfaces > one general interface:
```csharp
public interface IProducer<out T> { T Produce(); }
public interface IConsumer<in T> { void Consume(T item); }
// Segregated instead of: IHandler<T> { T Get(); void Set(T item); }
```

#### Dependency Inversion Principle (DIP)
Depend on abstractions, not concretions:
```csharp
// High-level depends on abstraction
public class DemoRunner
{
    private readonly IProducer<Animal> _producer;
    public DemoRunner(IProducer<Animal> producer) => _producer = producer;
}
```

---

## 📦 Module Structure

### Namespace Organization

```
AdvancedCsharpConcepts
├── Beginner
│   ├── Override_Upcast_Downcast
│   │   ├── Vehicle.cs
│   │   ├── Car.cs
│   │   └── Bike.cs
│   ├── Polymorphism_AssignCompatibility
│   │   ├── Animal.cs
│   │   ├── Mammal.cs
│   │   ├── Cat.cs
│   │   ├── Dog.cs
│   │   └── AssignmentCompatibility.cs
│   └── Upcast_Downcast
│       ├── Employee.cs
│       └── Manager.cs
├── Intermediate
│   ├── BoxingUnboxing
│   │   └── BoxingUnboxing.cs
│   └── CovarianceContravariance
│       ├── Covariance.cs
│       └── CovarianceContravariance.cs
├── Advanced
│   ├── ExplicitImplicitConversion
│   │   ├── Temperature.cs
│   │   └── ExplicitImplicitConversion.cs
│   └── GenericCovarianceContravariance
│       ├── IProducer.cs
│       ├── IConsumer.cs
│       ├── AnimalProducer.cs
│       ├── CatProducer.cs
│       ├── DogProducer.cs
│       └── AnimalConsumer.cs
└── Program.cs
```

---

## 🔄 Data Flow Diagrams

### Polymorphic Dispatch Flow

```
┌────────────────┐
│  List<Vehicle> │
└────────┬───────┘
         │ foreach
         ▼
    ┌────────────┐
    │  vehicle   │
    └────┬───────┘
         │ .Drive()
         ▼
    ┌────────────┐
    │  Virtual   │
    │ Dispatch   │
    └────┬───────┘
         │
    ┌────┴─────┐
    │          │
┌───▼──┐   ┌──▼───┐
│ Car  │   │ Bike │
│.Drive│   │.Drive│
└──────┘   └──────┘
```

### Variance Conversion Flow

```
Covariance (out T):
  IProducer<Dog>
       │
       │ Upcast (implicit)
       ▼
  IProducer<Animal>

Contravariance (in T):
  IConsumer<Animal>
       │
       │ Downcast (implicit)
       ▼
  IConsumer<Dog>
```

---

## 🎯 Design Patterns

### 1. Template Method Pattern
```csharp
public abstract class Vehicle
{
    public void StartJourney()
    {
        StartEngine();
        Drive(); // Template method
        StopEngine();
    }

    protected abstract void Drive();
}
```

### 2. Strategy Pattern (Variance)
```csharp
public interface IProducer<out T>
{
    T Produce(); // Strategy for producing T
}
```

### 3. Factory Pattern (Planned)
```csharp
public interface IVehicleFactory
{
    Vehicle CreateVehicle();
}
```

---

## ⚡ Performance Considerations

### Virtual Method Call Overhead

```
Direct Method Call:    ~0.3 ns
Virtual Method Call:   ~0.8 ns  (2.7x slower)
Interface Call:        ~1.2 ns  (4x slower)

Trade-off: Flexibility vs Performance
```

### Boxing Allocation Impact

```
Generic (no boxing):   0 bytes allocated
Boxing value type:     24 bytes per operation

Recommendation: Use generics to avoid boxing
```

### Memory Layout

```
Value Types (struct):
  - Stack allocated (if local)
  - Inline in containing type
  - No GC pressure

Reference Types (class):
  - Heap allocated
  - Pointer overhead (8/16 bytes)
  - GC tracked
```

---

## 🔒 Security Architecture

### Input Validation
All public APIs validate inputs to prevent:
- Null reference exceptions
- Type casting failures
- Invalid conversions

### Type Safety
```csharp
// Unsafe cast
Car car = (Car)vehicle; // Can throw InvalidCastException

// Safe cast with 'as'
Car? car = vehicle as Car; // Returns null if cast fails

// Safe cast with pattern matching
if (vehicle is Car car)
{
    car.Honk();
}
```

---

## 📊 Scalability Patterns

### Future Enhancements

1. **Dependency Injection**
   - Microsoft.Extensions.DependencyInjection
   - Service lifetimes (Singleton, Scoped, Transient)

2. **Caching Strategy**
   - Memory cache for frequently accessed data
   - Distributed cache for multi-instance scenarios

3. **Async/Await Patterns**
   - Asynchronous producers/consumers
   - ValueTask<T> for high-performance scenarios

4. **Parallel Processing**
   - Parallel.ForEach for batch processing
   - PLINQ for data-parallel operations

---

## 🧪 Testing Strategy

### Test Pyramid

```
         ┌─────────┐
         │   E2E   │  (10%)
         └─────────┘
       ┌─────────────┐
       │ Integration │  (20%)
       └─────────────┘
    ┌──────────────────┐
    │   Unit Tests     │  (70%)
    └──────────────────┘
```

### Test Coverage Targets

- **Unit Tests**: >90% code coverage
- **Branch Coverage**: >85%
- **Mutation Score**: >80%
- **Performance Tests**: All critical paths

---

## 📈 Monitoring & Observability

### Logging Architecture (Planned)

```
┌─────────────┐
│ Application │
└──────┬──────┘
       │ Serilog
       ▼
┌──────────────┐
│  Log Sinks   │
│              │
│ - Console    │
│ - File       │
│ - Seq        │
│ - Elasticsearch │
└──────────────┘
```

### Metrics Collection (Planned)

```
Application Metrics:
  - Method execution time
  - Boxing/unboxing frequency
  - Virtual dispatch count
  - Memory allocations
  - GC collections
```

---

## 🚀 Deployment Architecture

### Container Strategy

```
┌──────────────────────────────────────┐
│   Multi-Stage Docker Build           │
│                                      │
│  Stage 1: SDK (Build & Test)        │
│  Stage 2: Publish (Optimized)       │
│  Stage 3: Runtime (Minimal)         │
│                                      │
│  Final Image: ~100 MB (Alpine)      │
└──────────────────────────────────────┘
```

### Kubernetes Deployment (Planned)

```
┌─────────────────────────────────────┐
│       Kubernetes Cluster            │
│                                     │
│  ┌───────────────────────────────┐ │
│  │      Deployment               │ │
│  │  - Replicas: 3                │ │
│  │  - Rolling Update             │ │
│  │  - Health Checks              │ │
│  └───────────────────────────────┘ │
│                                     │
│  ┌───────────────────────────────┐ │
│  │      Service                  │ │
│  │  - LoadBalancer               │ │
│  │  - ClusterIP                  │ │
│  └───────────────────────────────┘ │
│                                     │
└─────────────────────────────────────┘
```

---

## 📚 Architecture Decision Records (ADRs)

### ADR-001: .NET 8 Upgrade

**Status**: Accepted

**Context**: Need modern .NET features and long-term support

**Decision**: Upgrade from .NET 6 to .NET 8 LTS

**Consequences**:
- ✅ Performance improvements (~15-25% faster)
- ✅ C# 12 features (primary constructors, collection expressions)
- ✅ LTS support until November 2026
- ❌ Requires SDK 8.0.100+

### ADR-002: Multi-Stage Docker Builds

**Status**: Accepted

**Context**: Need minimal production images

**Decision**: Use multi-stage Docker builds with Alpine base

**Consequences**:
- ✅ Reduced image size (~100MB vs ~200MB)
- ✅ Faster deployments
- ✅ Better security (minimal attack surface)
- ❌ Slightly longer build times

### ADR-003: Code Quality Analyzers

**Status**: Accepted

**Context**: Enforce coding standards and best practices

**Decision**: Use StyleCop, Roslynator, and SonarAnalyzer

**Consequences**:
- ✅ Consistent code style
- ✅ Early detection of code smells
- ✅ Security vulnerability detection
- ❌ Longer build times
- ❌ Initial configuration effort

---

## 🔮 Future Architecture Evolution

### Phase 2: Microservices (If Needed)

```
┌────────────────────────────────────────────────┐
│              API Gateway                       │
└────────────┬───────────────────────────────────┘
             │
    ┌────────┼────────┬────────────┐
    │        │        │            │
┌───▼───┐ ┌─▼──┐ ┌───▼───┐  ┌────▼────┐
│ Demo  │ │Bench│ │ Docs  │  │ Metrics │
│Service│ │Mark │ │Service│  │ Service │
└───────┘ └────┘ └───────┘  └─────────┘
```

### Phase 3: Event-Driven Architecture

```
┌────────────┐     ┌─────────────┐
│  Producer  │────▶│ Event Bus   │
└────────────┘     │  (RabbitMQ) │
                   └──────┬──────┘
                          │
                   ┌──────▼──────┐
                   │  Consumer   │
                   └─────────────┘
```

---

**Document Version**: 1.0.0
**Last Updated**: 2025-01-14
**Author**: Doğa Aydın
**Status**: Living Document

---

*This architecture document evolves with the project. See CHANGELOG.md for version history.*
