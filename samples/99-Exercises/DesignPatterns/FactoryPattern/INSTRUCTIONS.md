# 🎯 Exercise: Factory Pattern - Vehicle Creation System

**Difficulty:** 🟡 Intermediate
**Estimated Time:** 20-30 minutes
**Tests:** 5 tests

---

## 📚 Learning Objectives

- ✅ Understand Factory design pattern
- ✅ Learn when to use factory methods
- ✅ Practice object creation abstraction
- ✅ Apply polymorphism in object creation

---

## 🎯 Problem Statement

You're building a **vehicle rental system** that creates different types of vehicles (Car, Motorcycle, Truck). Each vehicle type has different properties and behavior.

**Bad Approach:**
```csharp
// Client code needs to know about all concrete classes
var vehicle = new Car(4, "Sedan");
var vehicle2 = new Motorcycle(2, true);
// Adding new vehicle type = change all client code! ❌
```

**Good Approach (Factory Pattern):**
```csharp
// Client only knows about IVehicle and VehicleFactory
IVehicle vehicle = VehicleFactory.CreateVehicle(VehicleType.Car);
// Adding new vehicle = just update factory! ✅
```

---

## 📋 Your Task

### Step 1: Complete `VehicleFactory` Class

**File:** `VehicleFactory.cs`

Implement the `CreateVehicle` method that:
1. Takes a `VehicleType` enum
2. Returns the appropriate vehicle instance
3. Throws exception for unknown types

### Step 2: Run Tests

```bash
cd samples/99-Exercises/DesignPatterns/FactoryPattern
dotnet test
```

**Expected:** All 5 tests pass

---

## 💡 Hints

```csharp
public static IVehicle CreateVehicle(VehicleType type)
{
    return type switch
    {
        VehicleType.Car => new Car(),
        // TODO: Add Motorcycle case
        // TODO: Add Truck case
        _ => throw new ArgumentException($"Unknown vehicle type: {type}")
    };
}
```

---

## ✅ Acceptance Criteria

1. ✅ `CreateVehicle` returns `Car` for `VehicleType.Car`
2. ✅ `CreateVehicle` returns `Motorcycle` for `VehicleType.Motorcycle`
3. ✅ `CreateVehicle` returns `Truck` for `VehicleType.Truck`
4. ✅ Throws exception for invalid types
5. ✅ All tests pass

---

## 🎓 Key Concepts

**Factory Pattern** encapsulates object creation, allowing subclasses or factory methods to decide which class to instantiate.

**Benefits:**
- ✅ Centralized creation logic
- ✅ Easy to add new types
- ✅ Loose coupling
- ✅ Single Responsibility Principle

**When to use:**
- Multiple related classes
- Complex creation logic
- Need to abstract object creation
- Runtime type determination

---

Check `SOLUTION.md` if stuck for more than 15 minutes.
