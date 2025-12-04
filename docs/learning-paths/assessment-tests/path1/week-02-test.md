# Week 2 Assessment Test - Inheritance and Assignment Compatibility

**Path**: 1 - Junior Developer
**Week**: 2
**Topics**: Inheritance, Assignment compatibility, Upcasting/Downcasting, Liskov Substitution
**Duration**: 35 minutes
**Passing Score**: 70% (7/10 correct)
**Total Points**: 10

---

## Section 1: Multiple Choice (5 questions, 1 point each)

### Question 1
What is upcasting?
a) Converting a derived class reference to a base class reference
b) Converting a base class reference to a derived class reference
c) Converting between two unrelated classes
d) Casting to a higher numeric type

### Question 2
Which operator safely checks if a cast is possible before performing it?
a) `as`
b) `is`
c) `typeof`
d) `cast`

### Question 3
What does the Liskov Substitution Principle state?
a) Derived classes must have more features than base classes
b) Objects of a derived class should be replaceable with objects of the base class
c) Base classes should inherit from derived classes
d) All classes must implement interfaces

### Question 4
When does downcasting require an explicit cast?
a) Always
b) Never
c) Only when casting to interfaces
d) Only in debug mode

### Question 5
What happens when you downcast incorrectly without checking?
a) Compile error
b) Runtime exception (InvalidCastException)
c) Returns null
d) Nothing happens

---

## Section 2: Short Answer (3 questions, 1.5 points each)

### Question 6
Explain the difference between `is` and `as` operators for casting. Give an example of when to use each.

### Question 7
Why is assignment compatibility important in collections? Give an example with `List<Animal>`.

### Question 8
What is method hiding and how does it differ from method overriding?

---

## Section 3: Code Analysis (1.5 points)

### Question 9
Analyze this code and identify potential issues:

```csharp
Animal animal = new Dog();
Dog dog = animal; // Line A
dog.Fetch(); // Line B

Cat cat = (Cat)animal; // Line C
cat.Meow(); // Line D
```

Identify which lines will cause errors and explain why.

---

## Answer Key

### Section 1
1. **a** - Upcasting is implicit, derived → base
2. **b** - `is` operator checks type before casting
3. **b** - LSP: Subtypes must be substitutable for base types
4. **a** - Downcasting always requires explicit cast
5. **b** - InvalidCastException at runtime if cast fails

### Section 2
6. `is` returns bool (type check), `as` returns null if cast fails. Use `is` for conditional logic, `as` when null is acceptable.
7. Assignment compatibility allows storing different types in one collection: `List<Animal>` can hold Dogs, Cats, Birds, enabling polymorphic behavior.
8. Method hiding (with `new`) creates a separate method; overriding (with `override`) replaces the base implementation. Hiding breaks polymorphism.

### Section 3
9.
- **Line A**: Compile error - implicit downcast not allowed
- **Line B**: Won't execute due to Line A
- **Line C**: Runtime error - InvalidCastException (Dog cannot be cast to Cat)
- **Line D**: Won't execute due to Line C

**Fix**: Use `as` or `is` for safe casting:
```csharp
if (animal is Dog dog) {
    dog.Fetch();
}
```

---

**Study Resources**: `samples/01-Beginner/AssignmentCompatibility/`, `samples/01-Beginner/Upcasting-Downcasting/`
