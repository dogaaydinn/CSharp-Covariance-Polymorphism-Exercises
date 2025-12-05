# Builder Pattern

## Problem
Implement a fluent builder for creating complex objects.

## Example
```csharp
var pizza = new PizzaBuilder()
    .WithSize("Large")
    .WithCrust("Thin")
    .AddTopping("Cheese")
    .AddTopping("Pepperoni")
    .Build();
```

## Requirements
- Fluent API (method chaining)
- Validation in Build()
- Immutable product

## Use Cases
- Building complex DTOs
- Test data builders
- Configuration objects
