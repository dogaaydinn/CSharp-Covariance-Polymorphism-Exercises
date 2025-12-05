# Expression Trees

## Problem
Build dynamic LINQ queries using Expression Trees.

## Example
```csharp
// Build: products.Where(p => p.Price > 100)
var param = Expression.Parameter(typeof(Product), "p");
var property = Expression.Property(param, "Price");
var constant = Expression.Constant(100m);
var greaterThan = Expression.GreaterThan(property, constant);
var lambda = Expression.Lambda<Func<Product, bool>>(greaterThan, param);

var filtered = products.Where(lambda.Compile());
```

## Use Cases
- Dynamic queries (Entity Framework)
- Mapping frameworks (AutoMapper)
- Validation frameworks
