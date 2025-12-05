# Dependency Injection

## Problem
Build a simple IoC container that resolves dependencies.

## Example
```csharp
var container = new DIContainer();
container.Register<ILogger, ConsoleLogger>();
container.Register<IDatabase, SqlDatabase>();
container.Register<UserService>();

var service = container.Resolve<UserService>();
// UserService receives ILogger and IDatabase automatically
```

## Requirements
- Constructor injection
- Singleton and Transient lifetimes
- Circular dependency detection

## ASP.NET Core
```csharp
services.AddTransient<ILogger, ConsoleLogger>();
services.AddSingleton<IDatabase, SqlDatabase>();
```
