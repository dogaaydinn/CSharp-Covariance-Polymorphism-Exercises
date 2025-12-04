# Weeks 8-24 Assessment Tests - Quick Reference Guide

> **Note**: This file contains condensed versions of weeks 8-24 tests. Each follows the same format as weeks 1-7.

---

## Week 8: Functional Programming

**Topics**: Lambdas, Func<T>, Action<T>, Closures
**Key Questions**:
- What's the difference between Func and Action?
- Explain closures and captured variables
- Write higher-order function examples
- Code: Fix closure in loop problem

---

## Week 9: Generic Covariance

**Topics**: IEnumerable<out T>, Covariant interfaces
**Key Questions**:
- What does `out` keyword mean?
- Why is IEnumerable<Cat> assignable to IEnumerable<Animal>?
- When is covariance safe?
- Code: Design covariant interface

**Resources**: `samples/02-Intermediate/CovarianceContravariance/`, `samples/99-Exercises/Generics/01-Covariance/`

---

## Week 10: Generic Contravariance

**Topics**: IComparer<in T>, Action<in T>, Contravariance
**Key Questions**:
- What does `in` keyword mean?
- Why is IComparer<Animal> assignable to IComparer<Cat>?
- Difference between covariance and contravariance
- Code: Implement contravariant comparer

**Resources**: `samples/99-Exercises/Generics/02-Contravariance/`

---

## Week 11: Generic Constraints

**Topics**: where T : class, struct, new(), interface constraints
**Key Questions**:
- List all generic constraint types
- When to use `where T : class` vs `where T : struct`?
- What's `where T : new()` for?
- Code: Design Repository<T> with constraints

**Resources**: `samples/99-Exercises/Generics/03-GenericConstraints/`

---

## Week 12: Builder Pattern

**Topics**: Fluent interface, Method chaining, Director pattern
**Key Questions**:
- What problem does Builder solve?
- How does fluent interface work (`return this`)?
- Builder vs Constructor with many parameters?
- Code: Implement builder for complex object

**Resources**: `samples/99-Exercises/DesignPatterns/01-Builder/`

---

## Week 13: Binary Search Algorithm

**Topics**: O(log n), Divide-and-conquer, Modified binary search
**Key Questions**:
- Explain binary search algorithm
- Time complexity: why O(log n)?
- How to find first/last occurrence?
- Code: Implement binary search iteratively

**Resources**: `samples/99-Exercises/Algorithms/01-BinarySearch/`

---

## Week 14: QuickSort Algorithm

**Topics**: Lomuto partition, Pivot selection, QuickSelect
**Key Questions**:
- Explain QuickSort algorithm
- How does partition work?
- Best vs worst case: O(n log n) vs O(n²)?
- Code: Implement partition function

**Resources**: `samples/99-Exercises/Algorithms/02-QuickSort/`

---

## Week 15: MergeSort Algorithm

**Topics**: Divide-and-conquer, Stable sorting, O(n log n)
**Key Questions**:
- How does MergeSort differ from QuickSort?
- What's stable sorting?
- Space complexity: O(n) vs O(1)?
- Code: Implement merge function

**Resources**: `samples/99-Exercises/Algorithms/03-MergeSort/`

---

## Week 16: Data Structures

**Topics**: Stack, Queue, Linked List, Complexity
**Key Questions**:
- When to use Stack vs Queue?
- Array vs Linked List trade-offs?
- What's Big O notation?
- Code: Implement Stack<T>

---

## Week 17: Decorator Pattern

**Topics**: Component wrapping, Dynamic behavior, Chaining
**Key Questions**:
- What problem does Decorator solve?
- Decorator vs Inheritance?
- How to chain multiple decorators?
- Code: Implement decorator for IDataSource

**Resources**: `samples/99-Exercises/DesignPatterns/03-Decorator/`

---

## Week 18: SOLID - SRP & OCP

**Topics**: Single Responsibility, Open/Closed Principle
**Key Questions**:
- What's Single Responsibility Principle?
- Give example of SRP violation
- What's Open/Closed Principle?
- Code: Refactor class to follow SRP

---

## Week 19: SOLID - LSP & ISP

**Topics**: Liskov Substitution, Interface Segregation
**Key Questions**:
- Explain Liskov Substitution Principle
- Give example of LSP violation (Square/Rectangle)
- What's Interface Segregation?
- Code: Split fat interface

---

## Week 20: SOLID - DIP

**Topics**: Dependency Inversion, Dependency Injection
**Key Questions**:
- What's Dependency Inversion Principle?
- High-level vs low-level modules?
- What's Dependency Injection?
- Code: Refactor to use DI

---

## Week 21: Observer Pattern

**Topics**: IObservable<T>, IObserver<T>, Event-driven
**Key Questions**:
- What problem does Observer solve?
- IObservable vs C# events?
- How to manage subscription lifecycle?
- Code: Implement observer for events

**Resources**: `samples/99-Exercises/DesignPatterns/02-Observer/`

---

## Week 22: ASP.NET Core Basics

**Topics**: Controllers, Routing, Dependency Injection
**Key Questions**:
- What's MVC pattern?
- How does routing work?
- What's middleware pipeline?
- Code: Create simple REST endpoint

---

## Week 23: Entity Framework Core

**Topics**: DbContext, Migrations, LINQ to Entities
**Key Questions**:
- What's ORM?
- How do migrations work?
- Eager vs lazy loading?
- Code: Define entity relationship

---

## Week 24: Authentication & Authorization

**Topics**: JWT, Identity, Claims, Policies
**Key Questions**:
- What's difference between authentication and authorization?
- How does JWT work?
- What are claims?
- Code: Implement JWT authentication

---

## Test Format for Each Week

Each week's actual test file follows this structure:

**Duration**: 30-45 minutes
**Passing**: 70%
**Sections**:
1. Multiple Choice (5 questions, 5 points)
2. Short Answer (3 questions, 4.5 points)
3. Code Analysis/Implementation (1.5 points)

**Total**: 10 points per test

---

## Note on Implementation

Full detailed tests for weeks 8-24 can be generated using the template from weeks 1-7. Each test should:
- Cover that week's specific topics
- Reference appropriate samples from repo
- Include code examples from exercises
- Provide comprehensive answer keys
- Link to study resources

**Priority**: Create full tests as needed based on student progress through curriculum.

---

*For full detailed tests, see individual week files (to be created as needed)*
