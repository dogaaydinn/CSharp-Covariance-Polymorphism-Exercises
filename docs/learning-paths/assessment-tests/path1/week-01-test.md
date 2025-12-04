# Week 1 Assessment Test - C# Basics and Polymorphism Fundamentals

**Path**: 1 - Junior Developer
**Week**: 1
**Topics**: C# syntax, OOP basics, Virtual methods, Abstract classes, Interfaces
**Duration**: 30 minutes
**Passing Score**: 70% (7/10 correct)
**Total Points**: 10

---

## Instructions

- Read each question carefully
- For multiple choice, select the best answer
- For code analysis, explain what's wrong and how to fix it
- For short answer, be concise but complete
- No external resources allowed during the test

---

## Section 1: Multiple Choice (5 questions, 1 point each)

### Question 1
What keyword is used to allow a method to be overridden in a derived class?

a) `override`
b) `virtual`
c) `abstract`
d) `sealed`

**Points**: 1

---

### Question 2
Which of the following is TRUE about abstract classes in C#?

a) Abstract classes can be instantiated directly
b) Abstract classes cannot have concrete (implemented) methods
c) Abstract classes can have both abstract and concrete methods
d) Abstract classes cannot have constructors

**Points**: 1

---

### Question 3
What is polymorphism?

a) The ability to create multiple classes with the same name
b) The ability of different classes to respond to the same method call in different ways
c) The process of hiding implementation details
d) The ability to inherit from multiple base classes

**Points**: 1

---

### Question 4
What happens if you try to call a method marked as `abstract` from the base class?

a) It executes the base class implementation
b) It throws a compile-time error if not overridden in derived class
c) It does nothing
d) It calls the derived class implementation automatically

**Points**: 1

---

### Question 5
Which statement is TRUE about interfaces?

a) Interfaces can contain implementation code
b) A class can implement multiple interfaces
c) Interfaces can have constructors
d) Interfaces can contain fields

**Points**: 1

---

## Section 2: Short Answer (3 questions, 1.5 points each)

### Question 6
Explain the difference between `virtual` and `abstract` methods. When would you use each?

**Expected Answer Length**: 3-4 sentences

**Points**: 1.5

---

### Question 7
In the Animal/Cat/Dog hierarchy, why is it important that derived classes can be stored in base class references (e.g., `Animal animal = new Dog();`)? Give a practical example.

**Expected Answer Length**: 2-3 sentences

**Points**: 1.5

---

### Question 8
What is the purpose of the `override` keyword? What happens if you forget to use it when overriding a virtual method?

**Expected Answer Length**: 2-3 sentences

**Points**: 1.5

---

## Section 3: Code Analysis (1.5 points)

### Question 9
Analyze the following code and identify what's wrong. Explain the error and provide the corrected code.

```csharp
public class Animal
{
    public void MakeSound()
    {
        Console.WriteLine("Some sound");
    }
}

public class Dog : Animal
{
    public void MakeSound()
    {
        Console.WriteLine("Woof!");
    }
}

// Usage
Animal animal = new Dog();
animal.MakeSound(); // What does this print?
```

**What's wrong?** _(Explain the issue)_

**Corrected code:** _(Provide the fix)_

**Points**: 1.5

---

## Answer Key

### Section 1: Multiple Choice

**Question 1**: **b) `virtual`**
*Explanation*: The `virtual` keyword in the base class allows the method to be overridden in derived classes.

**Question 2**: **c) Abstract classes can have both abstract and concrete methods**
*Explanation*: Abstract classes can contain both abstract methods (no implementation) and concrete methods (with implementation).

**Question 3**: **b) The ability of different classes to respond to the same method call in different ways**
*Explanation*: Polymorphism allows objects of different types to be treated uniformly while exhibiting their specific behavior.

**Question 4**: **b) It throws a compile-time error if not overridden in derived class**
*Explanation*: Abstract methods have no implementation in the base class and MUST be overridden in any non-abstract derived class.

**Question 5**: **b) A class can implement multiple interfaces**
*Explanation*: C# supports multiple interface implementation but not multiple class inheritance.

---

### Section 2: Short Answer

**Question 6**: Expected answer:
- `virtual` methods have a default implementation in the base class that can be optionally overridden
- `abstract` methods have NO implementation and MUST be overridden in derived classes
- Use `virtual` when you want to provide a default behavior that can be customized
- Use `abstract` when every derived class must provide its own implementation

**Question 7**: Expected answer:
- Storing derived classes in base class references enables polymorphism
- This allows you to treat different types uniformly in collections (e.g., `List<Animal>` containing Dogs, Cats, Birds)
- Example: A zoo application can process all animals in a single loop without knowing their specific types

**Question 8**: Expected answer:
- The `override` keyword explicitly indicates that a method is overriding a base class virtual/abstract method
- Without `override`, you're creating a new method that hides the base method (method hiding with `new`)
- This can lead to unexpected behavior where the base class method is called instead of the derived class method

---

### Section 3: Code Analysis

**Question 9**:

**What's wrong?**
- The `MakeSound()` method in the `Animal` class is not marked as `virtual`
- The `MakeSound()` method in the `Dog` class is not marked with `override`
- Result: `animal.MakeSound()` will print "Some sound" instead of "Woof!" because the Dog method is hiding the Animal method, not overriding it

**Corrected code:**
```csharp
public class Animal
{
    public virtual void MakeSound() // Added 'virtual'
    {
        Console.WriteLine("Some sound");
    }
}

public class Dog : Animal
{
    public override void MakeSound() // Added 'override'
    {
        Console.WriteLine("Woof!");
    }
}

// Usage
Animal animal = new Dog();
animal.MakeSound(); // Now correctly prints "Woof!"
```

---

## Grading Rubric

| Section | Points Available | Criteria |
|---------|-----------------|----------|
| Multiple Choice | 5 | 1 point per correct answer |
| Short Answer Q6 | 1.5 | Full: All points covered. Partial: 0.75 for incomplete. Wrong: 0 |
| Short Answer Q7 | 1.5 | Full: Practical example given. Partial: 0.75 for concept only. Wrong: 0 |
| Short Answer Q8 | 1.5 | Full: Both parts explained. Partial: 1.0 for one part. Wrong: 0 |
| Code Analysis | 1.5 | Full: Correct diagnosis and fix. Partial: 1.0 for diagnosis only. Wrong: 0 |
| **Total** | **10** | **Pass: 7 points (70%)** |

---

## Study Resources

If you didn't pass, review these materials:
- 📁 `samples/01-Beginner/PolymorphismBasics/01_SimplePolymorphism.cs`
- 📁 `samples/01-Beginner/PolymorphismBasics/02_AbstractClasses.cs`
- 📁 `samples/01-Beginner/PolymorphismBasics/03_InterfacePolymorphism.cs`
- 📄 `samples/01-Beginner/PolymorphismBasics/WHY_THIS_PATTERN.md`

---

*Test Version: 1.0*
*Last Updated: 2025-12-02*
