# Month 1 Comprehensive Assessment - C# Fundamentals & OOP

**Path**: 1 - Junior Developer
**Month**: 1 (Weeks 1-4)
**Topics**: C# syntax, OOP, Polymorphism, Inheritance, Casting, Collections, Boxing/Unboxing
**Duration**: 60 minutes
**Passing Score**: 80% (16/20 correct)
**Total Points**: 20

---

## Section 1: Multiple Choice (10 questions, 0.5 points each = 5 points)

1. What's the correct order of access modifiers from most to least restrictive?
   - a) private, protected, internal, public
   - b) public, internal, protected, private
   - c) private, internal, protected, public
   - d) private, protected, public, internal

2. Which allows a method to be overridden?
   - a) abstract or virtual
   - b) sealed or static
   - c) readonly or const
   - d) ref or out

3. What's true about abstract classes?
   - a) Cannot have constructors
   - b) Can be instantiated
   - c) Must have at least one abstract member
   - d) Can have both abstract and concrete members

4. Upcasting is:
   - a) Always explicit
   - b) Always implicit and safe
   - c) Not allowed in C#
   - d) Only works with interfaces

5. Which operator checks type without casting?
   - a) as
   - b) is
   - c) typeof
   - d) cast

6. Array covariance allows:
   - a) Dog[] to Cat[]
   - b) Dog[] to Animal[]
   - c) List<Dog> to List<Animal>
   - d) All generic collections

7. What exception occurs with invalid array covariance?
   - a) NullReferenceException
   - b) ArrayTypeMismatchException
   - c) InvalidCastException
   - d) ArgumentException

8. Boxing converts:
   - a) Reference to value type
   - b) Value to reference type
   - c) Int to string
   - d) String to int

9. Which collection avoids boxing for value types?
   - a) ArrayList
   - b) Hashtable
   - c) List<T>
   - d) object[]

10. IEnumerable<T> provides:
    - a) Random access
    - b) Forward iteration
    - c) Sorting
    - d) Indexing

---

## Section 2: Short Answer (6 questions, 1.5 points each = 9 points)

11. Explain polymorphism with a real-world example. How does it improve code flexibility?

12. What's the difference between method overriding and method hiding? Give code examples of each.

13. Why is the Liskov Substitution Principle important? Give an example of a violation.

14. Explain array covariance in C#. Why is it both useful and potentially dangerous?

15. Describe the performance impact of boxing in a high-frequency loop (1 million iterations). How would you avoid it?

16. Compare and contrast `virtual` methods, `abstract` methods, and interface methods. When would you use each?

---

## Section 3: Code Analysis & Implementation (3 questions, 2 points each = 6 points)

17. **Fix this code** - Identify all errors and provide corrected version:
```csharp
public class Shape
{
    public void Draw()
    {
        Console.WriteLine("Drawing shape");
    }
}

public class Circle : Shape
{
    public void Draw()
    {
        Console.WriteLine("Drawing circle");
    }
}

// Usage
Shape shape = new Circle();
shape.Draw(); // What prints? What's the problem?
```

18. **Implement this** - Create an animal hierarchy with proper polymorphism:
```csharp
// Requirements:
// 1. Abstract Animal base class with Name property
// 2. Abstract MakeSound() method
// 3. Virtual Eat() method with default implementation
// 4. Dog and Cat derived classes
// 5. Demonstrate polymorphism in Main()
```

19. **Optimize this** - Refactor to avoid boxing:
```csharp
ArrayList numbers = new ArrayList();
for (int i = 0; i < 1000000; i++)
{
    numbers.Add(i);
    object obj = numbers[i];
    int value = (int)obj;
    Console.WriteLine(value);
}
```

---

## Answer Key

### Section 1: Multiple Choice
1. **a** | 2. **a** | 3. **d** | 4. **b** | 5. **b** | 6. **b** | 7. **b** | 8. **b** | 9. **c** | 10. **b**

### Section 2: Short Answer

**11. Polymorphism** (1.5 pts):
- Definition: Different objects responding to same method differently
- Example: Animal hierarchy where Dog.MakeSound() = "Woof", Cat.MakeSound() = "Meow"
- Flexibility: Can process List<Animal> without knowing specific types
- Extensibility: Add new animals without changing existing code

**12. Override vs Hiding** (1.5 pts):
```csharp
// Overriding (polymorphic)
class Base { public virtual void Method() {} }
class Derived : Base { public override void Method() {} }

// Hiding (not polymorphic)
class Base2 { public void Method() {} }
class Derived2 : Base2 { public new void Method() {} }
```
- Override: Replaces base implementation, works polymorphically
- Hiding: Creates separate method, breaks polymorphism

**13. LSP** (1.5 pts):
- LSP: Derived classes must be substitutable for base class
- Violation example: Square inheriting Rectangle
  - Rectangle has independent width/height setters
  - Square violates this (width = height always)
  - Breaks expectation, leads to bugs
- Importance: Ensures inheritance hierarchies are logically sound

**14. Array Covariance** (1.5 pts):
- Allows: `Animal[] animals = new Dog[5];` (reference types only)
- Useful: Store related types in one array
- Dangerous: Runtime exception if wrong type assigned
  - `animals[0] = new Cat();` throws ArrayTypeMismatchException
- Generic collections don't have this issue (invariant)

**15. Boxing Performance** (1.5 pts):
- 1M boxing operations = 1M heap allocations
- High GC pressure → frequent collections → pauses (100-500ms)
- Memory: Each box = 12-16 bytes overhead + value
- Avoidance: Use List<int> instead of ArrayList, generics over object

**16. Virtual/Abstract/Interface** (1.5 pts):
- **Virtual**: Optional override, has default implementation
  - Use: Common behavior, customizable
- **Abstract**: Mandatory override, no implementation
  - Use: Contract that derived classes must fulfill
- **Interface**: Contract only, multiple allowed
  - Use: Define capabilities, support multiple inheritance

### Section 3: Code Analysis

**17. Fix Shape/Circle** (2 pts):
**Problems**:
- Shape.Draw() not virtual → no polymorphism
- Circle.Draw() not override → hides instead
- Result: Prints "Drawing shape" not "Drawing circle"

**Fixed**:
```csharp
public class Shape
{
    public virtual void Draw() // Added virtual
    {
        Console.WriteLine("Drawing shape");
    }
}

public class Circle : Shape
{
    public override void Draw() // Added override
    {
        Console.WriteLine("Drawing circle");
    }
}

// Now correctly prints "Drawing circle"
```

**18. Animal Hierarchy** (2 pts):
```csharp
public abstract class Animal
{
    public string Name { get; set; }

    public abstract void MakeSound();

    public virtual void Eat()
    {
        Console.WriteLine($"{Name} is eating");
    }
}

public class Dog : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine($"{Name}: Woof!");
    }
}

public class Cat : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine($"{Name}: Meow!");
    }

    public override void Eat()
    {
        Console.WriteLine($"{Name} is eating fish");
    }
}

// Main
List<Animal> animals = new()
{
    new Dog { Name = "Buddy" },
    new Cat { Name = "Whiskers" }
};

foreach (var animal in animals)
{
    animal.MakeSound();
    animal.Eat();
}
```

**19. Optimize Boxing** (2 pts):
```csharp
List<int> numbers = new List<int>(); // Generic, no boxing
for (int i = 0; i < 1000000; i++)
{
    numbers.Add(i); // No boxing
    int value = numbers[i]; // No unboxing
    Console.WriteLine(value); // Still boxes for WriteLine
}

// Even better: avoid unnecessary operations
List<int> numbers = Enumerable.Range(0, 1000000).ToList();
```

**Performance Improvement**:
- ArrayList version: 3M allocations (add, get, WriteLine)
- List<int> version: 1M allocations (only WriteLine)
- 66% reduction in allocations

---

## Grading Rubric

| Section | Max Points | Criteria |
|---------|-----------|----------|
| Multiple Choice | 5 | 0.5 per correct answer |
| Short Answer (each) | 1.5 × 6 = 9 | Full: Complete answer. Partial: 0.75-1.0. Wrong: 0 |
| Code Fix (Q17) | 2 | Full: All errors found + fixed. Partial: 1.0. Wrong: 0 |
| Code Implement (Q18) | 2 | Full: All requirements met. Partial: 1.0-1.5. Wrong: 0 |
| Code Optimize (Q19) | 2 | Full: Correct + explanation. Partial: 1.0-1.5. Wrong: 0 |
| **Total** | **20** | **Pass: 16 points (80%)** |

---

## Next Steps

**If you passed (≥16 pts)**: Congratulations! Proceed to Month 2 (LINQ Mastery)

**If you didn't pass (<16 pts)**: Review weak areas:
- Score 0-5: Review all Month 1 materials
- Score 6-10: Focus on OOP and polymorphism
- Score 11-15: Practice code implementation

**Study Resources**:
- `samples/01-Beginner/` (all samples)
- `samples/02-Intermediate/ArrayCovariance/`, `BoxingUnboxing/`
- `samples/99-Exercises/LINQ/01-BasicQueries/` (for LINQ basics)

---

*Assessment Version: 1.0*
*Last Updated: 2025-12-02*
