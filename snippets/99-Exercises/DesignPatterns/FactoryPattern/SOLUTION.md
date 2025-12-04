# 💡 Solution: Factory Pattern - Vehicle Creation System

## Complete Solution

### VehicleFactory.cs

```csharp
namespace FactoryPattern;

public static class VehicleFactory
{
    public static IVehicle CreateVehicle(VehicleType type)
    {
        return type switch
        {
            VehicleType.Car => new Car(),
            VehicleType.Motorcycle => new Motorcycle(),
            VehicleType.Truck => new Truck(),
            _ => throw new ArgumentException($"Unknown vehicle type: {type}")
        };
    }
}
```

## Explanation

The Factory Pattern centralizes object creation logic. Instead of using `new Car()` throughout your codebase, you call `VehicleFactory.CreateVehicle(VehicleType.Car)`.

**Benefits:**
1. **Single point of change**: To modify creation logic, change only the factory
2. **Encapsulation**: Client doesn't need to know about concrete classes
3. **Flexibility**: Can return different subtypes based on configuration
4. **Testability**: Easy to mock the factory

**Switch Expression**:
- Modern C# 8.0+ syntax
- Cleaner than if-else chains
- Forces handling of all cases (with `_` default case)

**When to use Factory Pattern:**
- Creating objects with complex initialization
- Need to decide at runtime which class to instantiate
- Want to hide object creation details from client

Run `dotnet test` to verify your solution!
