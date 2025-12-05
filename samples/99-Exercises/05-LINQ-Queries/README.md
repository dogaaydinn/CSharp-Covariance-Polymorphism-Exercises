# LINQ Queries

## Problems
Solve these queries using LINQ:

1. Find all even numbers
2. Get top 3 highest values
3. Group by category and count
4. Join two lists
5. Calculate average by group

## Example
```csharp
var numbers = new[] { 1, 2, 3, 4, 5, 6 };
var evens = numbers.Where(n => n % 2 == 0);
```

## Key Operators
- Where, Select, OrderBy
- GroupBy, Join
- Count, Sum, Average
- Take, Skip
- Any, All, First
