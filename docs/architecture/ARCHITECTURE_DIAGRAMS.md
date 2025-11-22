# Architecture Diagrams

This document contains architecture diagrams for the C# Advanced Concepts project.

## Table of Contents

1. [Project Structure](#1-project-structure)
2. [Polymorphism Class Hierarchy](#2-polymorphism-class-hierarchy)
3. [Design Patterns Architecture](#3-design-patterns-architecture)
4. [Observability Architecture](#4-observability-architecture)
5. [High-Performance Data Flow](#5-high-performance-data-flow)
6. [Dependency Injection Container](#6-dependency-injection-container)

---

## 1. Project Structure

```mermaid
graph TD
    A[AdvancedCsharpConcepts.sln] --> B[AdvancedCsharpConcepts]
    A --> C[AdvancedCsharpConcepts.Tests]
    A --> D[AdvancedCsharpConcepts.IntegrationTests]

    B --> B1[Beginner]
    B --> B2[Intermediate]
    B --> B3[Advanced]

    B1 --> B1A[Polymorphism]
    B1 --> B1B[Upcast-Downcast]

    B2 --> B2A[Boxing/Unboxing]
    B2 --> B2B[Covariance/Contravariance]

    B3 --> B3A[DesignPatterns]
    B3 --> B3B[HighPerformance]
    B3 --> B3C[ModernCSharp]
    B3 --> B3D[Observability]
    B3 --> B3E[DependencyInjection]

    B3A --> B3A1[Factory]
    B3A --> B3A2[Builder]
    B3A --> B3A3[Strategy]
    B3A --> B3A4[Observer]
    B3A --> B3A5[Decorator]

    B3B --> B3B1[SIMD]
    B3B --> B3B2[Span/Memory]
    B3B --> B3B3[Parallel Processing]

    B3D --> B3D1[Logging]
    B3D --> B3D2[Health Checks]
    B3D --> B3D3[Telemetry]

    style B fill:#e1f5ff
    style C fill:#fff4e1
    style D fill:#f0ffe1
    style B3A fill:#ffe1f5
    style B3B fill:#ffe1e1
    style B3D fill:#e1ffe1
```

---

## 2. Polymorphism Class Hierarchy

```mermaid
classDiagram
    class Animal {
        <<abstract>>
        +string Species
        +void MakeSound()*
        +void Eat()*
    }

    class Mammal {
        +string Species
        +void GiveBirth()
    }

    class Dog {
        +string Name
        +string Breed
        +void MakeSound()
        +void Bark()
    }

    class Cat {
        +string Name
        +string Color
        +void MakeSound()
        +void Meow()
    }

    class Employee {
        #int Age
        #string Name
        +void DisplayInfo()
    }

    class Manager {
        -int Bonus
        +void DownCast()
    }

    Animal <|-- Mammal
    Mammal <|-- Dog
    Mammal <|-- Cat
    Employee <|-- Manager

    note for Animal "Demonstrates polymorphism\nand abstract classes"
    note for Dog "Concrete implementation\nwith specific behavior"
    note for Manager "Shows upcasting and\ndowncasting examples"
```

---

## 3. Design Patterns Architecture

### Factory Pattern

```mermaid
classDiagram
    class IVehicle {
        <<interface>>
        +GetDescription() string
        +GetWheelCount() int
        +Start() void
    }

    class VehicleFactory {
        <<static>>
        +CreateVehicle(type, param) IVehicle
        -CreateCar(model) IVehicle
        -CreateMotorcycle(brand) IVehicle
        -CreateTruck(capacity) IVehicle
    }

    class Car {
        +string Model
        +GetDescription() string
        +GetWheelCount() int
        +Start() void
    }

    class Motorcycle {
        +string Brand
        +GetDescription() string
        +GetWheelCount() int
        +Start() void
    }

    class Truck {
        +int Capacity
        +GetDescription() string
        +GetWheelCount() int
        +Start() void
    }

    IVehicle <|.. Car
    IVehicle <|.. Motorcycle
    IVehicle <|.. Truck
    VehicleFactory ..> IVehicle : creates
    VehicleFactory ..> Car : creates
    VehicleFactory ..> Motorcycle : creates
    VehicleFactory ..> Truck : creates
```

### Strategy Pattern

```mermaid
classDiagram
    class IPaymentStrategy {
        <<interface>>
        +string Name
        +ProcessPayment(amount) bool
        +ValidatePayment(amount) bool
    }

    class ShoppingCart {
        -List~Item~ items
        -IPaymentStrategy paymentStrategy
        +AddItem(item, price) void
        +SetPaymentStrategy(strategy) void
        +GetTotal() decimal
        +Checkout() bool
    }

    class CreditCardPayment {
        +string CardNumber
        +string CVV
        +ProcessPayment(amount) bool
        +ValidatePayment(amount) bool
    }

    class PayPalPayment {
        +string Email
        +ProcessPayment(amount) bool
        +ValidatePayment(amount) bool
    }

    class CryptoPayment {
        +string WalletAddress
        +string CryptoType
        +ProcessPayment(amount) bool
        +ValidatePayment(amount) bool
    }

    IPaymentStrategy <|.. CreditCardPayment
    IPaymentStrategy <|.. PayPalPayment
    IPaymentStrategy <|.. CryptoPayment
    ShoppingCart o-- IPaymentStrategy : uses
```

### Observer Pattern

```mermaid
sequenceDiagram
    participant StockTicker
    participant MobileApp1
    participant MobileApp2
    participant EmailNotifier
    participant Dashboard

    MobileApp1->>StockTicker: Attach()
    MobileApp2->>StockTicker: Attach()
    EmailNotifier->>StockTicker: Attach()
    Dashboard->>StockTicker: Attach()

    Note over StockTicker: Stock price changes
    StockTicker->>StockTicker: UpdateStock("AAPL", 150.00)

    StockTicker->>MobileApp1: Update(StockData)
    StockTicker->>MobileApp2: Update(StockData)
    StockTicker->>EmailNotifier: Update(StockData)
    StockTicker->>Dashboard: Update(StockData)

    MobileApp1-->>StockTicker: Display notification
    EmailNotifier-->>StockTicker: Send email
    Dashboard-->>StockTicker: Update analytics

    MobileApp2->>StockTicker: Detach()

    Note over StockTicker: Another price change
    StockTicker->>MobileApp1: Update(StockData)
    StockTicker->>EmailNotifier: Update(StockData)
    StockTicker->>Dashboard: Update(StockData)
    Note over MobileApp2: Not notified (detached)
```

### Decorator Pattern

```mermaid
classDiagram
    class ICoffee {
        <<interface>>
        +GetDescription() string
        +GetCost() decimal
        +GetCalories() int
    }

    class Espresso {
        +GetDescription() string
        +GetCost() decimal
        +GetCalories() int
    }

    class CoffeeDecorator {
        <<abstract>>
        #ICoffee coffee
        +GetDescription() string
        +GetCost() decimal
        +GetCalories() int
    }

    class Milk {
        +GetDescription() string
        +GetCost() decimal
        +GetCalories() int
    }

    class Mocha {
        +GetDescription() string
        +GetCost() decimal
        +GetCalories() int
    }

    class Whip {
        +GetDescription() string
        +GetCost() decimal
        +GetCalories() int
    }

    ICoffee <|.. Espresso
    ICoffee <|.. CoffeeDecorator
    CoffeeDecorator <|-- Milk
    CoffeeDecorator <|-- Mocha
    CoffeeDecorator <|-- Whip
    CoffeeDecorator o-- ICoffee

    note for CoffeeDecorator "Wraps ICoffee and\nadds functionality"
```

---

## 4. Observability Architecture

```mermaid
graph TB
    subgraph Application
        A[Application Code]
        A --> L[Structured Logging]
        A --> M[Metrics]
        A --> T[Distributed Tracing]
        A --> H[Health Checks]
    end

    subgraph Logging
        L --> L1[Serilog]
        L1 --> L2[Console Sink]
        L1 --> L3[File Sink]
        L1 --> L4[Seq/ELK Sink]
    end

    subgraph Metrics
        M --> M1[System.Diagnostics.Metrics]
        M1 --> M2[Counter]
        M1 --> M3[Histogram]
        M1 --> M4[Gauge]
        M --> M5[OpenTelemetry Exporter]
        M5 --> M6[Prometheus]
    end

    subgraph Tracing
        T --> T1[Activity/Span]
        T1 --> T2[OpenTelemetry]
        T2 --> T3[Jaeger]
        T2 --> T4[Zipkin]
        T2 --> T5[Azure Monitor]
    end

    subgraph Health
        H --> H1[Database Check]
        H --> H2[API Check]
        H --> H3[Memory Check]
        H --> H4[Disk Check]
        H1 & H2 & H3 & H4 --> H5[/health endpoint]
    end

    subgraph Visualization
        M6 --> V1[Grafana]
        T3 --> V2[Jaeger UI]
        L4 --> V3[Kibana]
    end

    style Application fill:#e1f5ff
    style Logging fill:#ffe1e1
    style Metrics fill:#e1ffe1
    style Tracing fill:#ffe1f5
    style Health fill:#fff4e1
    style Visualization fill:#f0f0ff
```

---

## 5. High-Performance Data Flow

### SIMD Vectorization

```mermaid
graph LR
    subgraph "Scalar Processing (Old)"
        S1[Data1] --> SP1[Process]
        S2[Data2] --> SP2[Process]
        S3[Data3] --> SP3[Process]
        S4[Data4] --> SP4[Process]
        SP1 --> SR1[Result1]
        SP2 --> SR2[Result2]
        SP3 --> SR3[Result3]
        SP4 --> SR4[Result4]
    end

    subgraph "SIMD Processing (New)"
        V1[Data1, Data2, Data3, Data4] --> VP[Vector Process]
        VP --> VR[Result1, Result2, Result3, Result4]
    end

    style "Scalar Processing (Old)" fill:#ffe1e1
    style "SIMD Processing (New)" fill:#e1ffe1

    Note1[4x Sequential Operations] -.-> S1
    Note2[1x Parallel Operation\n4-8x Faster!] -.-> V1
```

### Parallel Processing

```mermaid
graph TD
    A[Input Data: 1M elements] --> B{Parallel.For}

    B --> C1[Thread 1:\nProcessing 0-250k]
    B --> C2[Thread 2:\nProcessing 250k-500k]
    B --> C3[Thread 3:\nProcessing 500k-750k]
    B --> C4[Thread 4:\nProcessing 750k-1M]

    C1 --> D1[Partial Result 1]
    C2 --> D2[Partial Result 2]
    C3 --> D3[Partial Result 3]
    C4 --> D4[Partial Result 4]

    D1 & D2 & D3 & D4 --> E[Combine Results]
    E --> F[Final Result]

    style A fill:#e1f5ff
    style C1 fill:#ffe1e1
    style C2 fill:#e1ffe1
    style C3 fill:#ffe1f5
    style C4 fill:#fff4e1
    style F fill:#e1ffe1
```

---

## 6. Dependency Injection Container

```mermaid
graph TD
    subgraph "Service Collection"
        SC[ServiceCollection]
        SC --> S1[Singleton: IDataRepository]
        SC --> S2[Transient: IDataProcessor]
        SC --> S3[Scoped: INotificationService]
    end

    subgraph "Service Provider"
        SP[ServiceProvider]
        SP --> R1[Resolve IDataRepository\nSingle Instance]
        SP --> R2[Resolve IDataProcessor\nNew Instance Each Time]
        SP --> R3[Resolve INotificationService\nOne Per Scope]
    end

    subgraph "Application"
        A[ApplicationService]
        A --> U1[Uses IDataRepository]
        A --> U2[Uses IDataProcessor]
        A --> U3[Uses INotificationService]
    end

    SC --> SP
    R1 --> U1
    R2 --> U2
    R3 --> U3

    style SC fill:#e1f5ff
    style SP fill:#ffe1e1
    style A fill:#e1ffe1
```

---

## 7. CI/CD Pipeline

```mermaid
graph LR
    A[Git Push] --> B[GitHub Actions]

    B --> C{Build & Test}

    C -->|Success| D[Code Quality]
    C -->|Fail| X1[❌ Notify]

    D --> E[Security Scan\nCodeQL]
    E --> F[Mutation Testing\nStryker]

    F --> G{All Checks Pass?}

    G -->|Yes| H[Docker Build]
    G -->|No| X2[❌ Fail Pipeline]

    H --> I[Push to Registry]
    I --> J[Deploy to Staging]
    J --> K[Integration Tests]

    K -->|Pass| L[✅ Ready for Production]
    K -->|Fail| X3[❌ Rollback]

    style A fill:#e1f5ff
    style C fill:#ffe1e1
    style E fill:#ffe1f5
    style G fill:#fff4e1
    style L fill:#e1ffe1
    style X1 fill:#ffcccc
    style X2 fill:#ffcccc
    style X3 fill:#ffcccc
```

---

## 8. Covariance and Contravariance Flow

```mermaid
graph TD
    subgraph "Covariance (out T) - Reading"
        COV1[IEnumerable~Dog~] -->|Implicit Conversion| COV2[IEnumerable~Animal~]
        COV2 -->|✅ Safe| COV3[Can only GET items\nRead-only operation]
    end

    subgraph "Contravariance (in T) - Writing"
        CONTRA1[Action~Animal~] -->|Implicit Conversion| CONTRA2[Action~Dog~]
        CONTRA2 -->|✅ Safe| CONTRA3[Can only ACCEPT items\nWrite-only operation]
    end

    subgraph "Invariance (T) - Both"
        INV1[List~Dog~] -.->|❌ Cannot Convert| INV2[List~Animal~]
        INV2 -->|Unsafe| INV3[Both read AND write\nNot type-safe]
    end

    style COV1 fill:#e1ffe1
    style COV2 fill:#e1ffe1
    style CONTRA1 fill:#ffe1f5
    style CONTRA2 fill:#ffe1f5
    style INV1 fill:#ffe1e1
    style INV2 fill:#ffe1e1

    Note1[More Derived → Less Derived\nNaturally converts UP] -.-> COV1
    Note2[Less Derived → More Derived\nNaturally accepts DOWN] -.-> CONTRA1
    Note3[No conversion allowed\nBoth directions unsafe] -.-> INV1
```

---

## Summary

These diagrams illustrate:

1. **Project Structure**: Clear organization from beginner to advanced topics
2. **Class Hierarchies**: Polymorphism and inheritance relationships
3. **Design Patterns**: Factory, Strategy, Observer, Decorator implementations
4. **Observability**: Logging, metrics, tracing, and health checks
5. **High-Performance**: SIMD vectorization and parallel processing
6. **Dependency Injection**: Service lifetimes and resolution
7. **CI/CD**: Automated build, test, and deployment pipeline
8. **Type Variance**: Covariance, contravariance, and invariance

All diagrams use **Mermaid** syntax and can be rendered in:
- GitHub README files
- GitLab documentation
- Markdown preview tools
- Documentation websites (DocFX, MkDocs, Docusaurus)
