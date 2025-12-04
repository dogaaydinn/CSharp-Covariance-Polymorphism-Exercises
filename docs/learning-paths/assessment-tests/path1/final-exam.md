# Path 1 Final Certification Exam - Junior Developer

**Path**: 1 - Zero to Junior Developer
**Duration**: 120 minutes (2 hours)
**Total Points**: 50
**Passing Score**: 85% (43/50 points)
**Topics**: All material from Months 1-6 (24 weeks)

---

## Exam Instructions

- This is a **comprehensive exam** covering all topics from the 6-month learning path
- You have **120 minutes** to complete all 50 questions
- You must score **85% or higher** (43 points) to pass and earn certification
- **No resources allowed** during the exam (closed book)
- Answer all questions - there is no penalty for wrong answers
- Review your answers before submitting

---

## Section 1: Multiple Choice (30 questions, 1 point each = 30 points)

### Month 1: C# Fundamentals & OOP (Questions 1-7)

1. What's true about abstract classes?
   - a) Cannot have constructors
   - b) Must be instantiated
   - c) Can have both abstract and concrete members
   - d) Cannot have properties

2. Which casting is always safe?
   - a) Downcasting | b) Upcasting | c) Both | d) Neither

3. Array covariance allows:
   - a) Dog[] to Animal[] | b) List<Dog> to List<Animal> | c) int[] to object[] | d) All

4. Boxing converts:
   - a) Reference to value | b) Value to reference | c) String to int | d) None

5. Which avoids boxing for value types?
   - a) ArrayList | b) List<T> | c) object[] | d) Hashtable

6. Virtual method allows:
   - a) Hiding only | b) Overriding | c) Neither | d) Static dispatch

7. Liskov Substitution Principle means:
   - a) Use interfaces only | b) Derived substitutable for base | c) Avoid inheritance | d) All classes abstract

### Month 2: LINQ Mastery (Questions 8-14)

8. Deferred execution means:
   - a) Never executes | b) Executes when enumerated | c) Executes immediately | d) Delayed start

9. Which forces immediate execution?
   - a) Where() | b) Select() | c) ToList() | d) OrderBy()

10. IGrouping<TKey, TElement> represents:
    - a) Dictionary | b) Group with key and elements | c) Array | d) Queue

11. Left outer join requires:
    - a) Join() | b) GroupJoin() + SelectMany() + DefaultIfEmpty() | c) LeftJoin() | d) Outer()

12. Closure captures:
    - a) Nothing | b) Variables from outer scope by reference | c) Only parameters | d) Global only

13. `SelectMany()` does:
    - a) Filters | b) Flattens nested collections | c) Projects single | d) Groups

14. `First()` vs `Single()` difference:
    - a) No difference | b) Single() throws if >1 element | c) First() throws always | d) Same behavior

### Month 3: Generics & Patterns (Questions 15-20)

15. `out` keyword in `IEnumerable<out T>` means:
    - a) Output parameter | b) Covariance | c) Contravariance | d) Optional

16. Covariance allows:
    - a) IEnumerable<Dog> to IEnumerable<Animal> | b) IEnumerable<Animal> to IEnumerable<Dog> | c) Both | d) Neither

17. `where T : new()` constraint requires:
    - a) Any constructor | b) Parameterless constructor | c) Static constructor | d) All constructors

18. Builder pattern primary benefit:
    - a) Performance | b) Readability for complex construction | c) Thread safety | d) Faster

19. Fluent interface uses:
    - a) Static methods | b) Method chaining with `return this` | c) Async methods | d) Properties only

20. Observer pattern relationship:
    - a) One-to-one | b) One-to-many | c) Many-to-one | d) None

### Month 4: Algorithms & Data Structures (Questions 21-25)

21. Binary search time complexity:
    - a) O(n) | b) O(log n) | c) O(n log n) | d) O(1)

22. QuickSort worst case:
    - a) O(n) | b) O(n log n) | c) O(n²) | d) O(log n)

23. MergeSort is:
    - a) Unstable, O(1) space | b) Stable, O(n) space | c) Unstable, O(n) space | d) Stable, O(1) space

24. Stack follows:
    - a) FIFO | b) LIFO | c) Random | d) Priority

25. Array access time:
    - a) O(1) | b) O(n) | c) O(log n) | d) O(n²)

### Month 5: SOLID Principles (Questions 26-30)

26. SRP means:
    - a) One method per class | b) One reason to change | c) Single inheritance | d) Static only

27. OCP means:
    - a) All methods public | b) Open for extension, closed for modification | c) Only constructors public | d) Private classes

28. LSP violation example:
    - a) Square inheriting Rectangle | b) Dog inheriting Animal | c) All inheritance | d) Interface implementation

29. ISP advocates:
    - a) One interface | b) Many specific interfaces over fat ones | c) No interfaces | d) Large interfaces

30. DIP means:
    - a) Avoid dependencies | b) Depend on abstractions, not concretions | c) Use static | d) No interfaces

---

## Section 2: Short Answer (10 questions, 1 point each = 10 points)

31. **Explain polymorphism** with a simple code example (3-4 lines).

32. **What's the difference between override and new (hiding)**? Give one-sentence explanation for each.

33. **Write LINQ query**: Get products where price > 50, group by category, get average price per category.

34. **Explain covariance**: Why is `IEnumerable<Dog>` assignable to `IEnumerable<Animal>`?

35. **Binary search prerequisite**: What must be true about the array/list?

36. **QuickSort partition**: What does the partition step accomplish in one sentence?

37. **Decorator pattern purpose**: What problem does it solve?

38. **Give one SRP violation example**: Name a class and what multiple responsibilities it has.

39. **Eager vs Lazy loading**: When would you use eager loading in EF Core?

40. **JWT structure**: Name the 3 parts of a JWT token.

---

## Section 3: Code Analysis (5 questions, 2 points each = 10 points)

### Question 41: Fix Polymorphism (2 points)

What's wrong with this code? Provide the fix.

```csharp
public class Animal
{
    public void MakeSound()
    {
        Console.WriteLine("Animal sound");
    }
}

public class Dog : Animal
{
    public void MakeSound()
    {
        Console.WriteLine("Woof!");
    }
}

Animal animal = new Dog();
animal.MakeSound(); // What prints?
```

**What prints?**: _______________

**Problem**: _______________

**Fix**: _______________

---

### Question 42: LINQ Deferred Execution (2 points)

What's the bug in this code? How do you fix it?

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };
var threshold = 2;
var query = numbers.Where(n => n > threshold);

threshold = 10; // Bug is here!

foreach (var n in query)
    Console.WriteLine(n); // What prints?
```

**What prints?**: _______________

**Why?**: _______________

**Fix**: _______________

---

### Question 43: Generic Constraints (2 points)

Complete the generic method with proper constraints:

```csharp
public class Repository<T> where _______________
{
    private List<T> _items = new List<T>();

    public void Add(T item)
    {
        _items.Add(item);
    }

    public T CreateNew()
    {
        return new T(); // Requires constraint!
    }
}

// T must:
// 1. Be a reference type
// 2. Have parameterless constructor
// 3. Implement IEntity interface

// Write constraints: _______________
```

---

### Question 44: SOLID Violation (2 points)

This class violates which SOLID principle(s)? Why?

```csharp
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }

    public void SaveToDatabase()
    {
        // SQL code
    }

    public void SendEmail(string message)
    {
        // Email code
    }

    public bool ValidateEmail()
    {
        // Validation code
    }
}
```

**Violates**: _______________

**Reason**: _______________

**How to fix**: _______________

---

### Question 45: Complexity Analysis (2 points)

What's the time complexity of each operation?

```csharp
// 1. Binary search in sorted array of n elements
Answer: _______________

// 2. QuickSort average case
Answer: _______________

// 3. Nested loops: for(i=0; i<n; i++) for(j=0; j<n; j++)
Answer: _______________

// 4. Array access: arr[5]
Answer: _______________

// 5. Linked list search for element
Answer: _______________
```

---

## Answer Key

### Section 1: Multiple Choice (30 points)

1. **c** - Can have both abstract and concrete members
2. **b** - Upcasting (derived to base)
3. **a** - Dog[] to Animal[] (arrays only)
4. **b** - Value to reference type
5. **b** - List<T> uses generics
6. **b** - Overriding
7. **b** - Derived substitutable for base

8. **b** - Executes when enumerated
9. **c** - ToList() (materializes query)
10. **b** - Group with key and elements
11. **b** - GroupJoin() + SelectMany() + DefaultIfEmpty()
12. **b** - Variables from outer scope by reference
13. **b** - Flattens nested collections
14. **b** - Single() throws if more than 1 element

15. **b** - Covariance
16. **a** - IEnumerable<Dog> to IEnumerable<Animal>
17. **b** - Parameterless constructor
18. **b** - Readability for complex construction
19. **b** - Method chaining with `return this`
20. **b** - One-to-many

21. **b** - O(log n)
22. **c** - O(n²)
23. **b** - Stable, O(n) space
24. **b** - LIFO (Last In First Out)
25. **a** - O(1)

26. **b** - One reason to change
27. **b** - Open for extension, closed for modification
28. **a** - Square inheriting Rectangle
29. **b** - Many specific interfaces
30. **b** - Depend on abstractions

### Section 2: Short Answer (10 points)

**31. Polymorphism**:
```csharp
Animal animal = new Dog();
animal.MakeSound(); // Calls Dog's implementation
// Runtime determines which method to call
```

**32. Override vs Hiding**:
- **Override**: Replaces base method, works polymorphically with `virtual`/`override`
- **Hiding**: Creates separate method with `new`, breaks polymorphism

**33. LINQ Query**:
```csharp
products
    .Where(p => p.Price > 50)
    .GroupBy(p => p.Category)
    .Select(g => new { Category = g.Key, AvgPrice = g.Average(p => p.Price) })
```

**34. Covariance**:
- Safe because IEnumerable<T> only produces T (read-only)
- Can read Dogs as Animals (upcasting is safe)
- Would be unsafe if could write (List<T> is invariant)

**35. Binary Search Prerequisite**:
- Array must be **sorted** in ascending or descending order

**36. Partition**:
- Rearranges array so pivot is in final position with smaller elements left, larger right

**37. Decorator Purpose**:
- Add responsibilities/behaviors to objects dynamically at runtime without affecting other objects

**38. SRP Violation**:
- **User class** handling: business logic, database persistence, email sending, validation
- Should separate into User (data), UserRepository (DB), EmailService (email), UserValidator (validation)

**39. Eager Loading**:
- Use when you **always** need related data to avoid N+1 query problem
- Example: Loading orders with customer info for display

**40. JWT Structure**:
1. Header (algorithm, type)
2. Payload (claims/data)
3. Signature (verification)

### Section 3: Code Analysis (10 points)

**41. Fix Polymorphism** (2 pts):
- **What prints**: "Animal sound"
- **Problem**: Base method not `virtual`, derived not `override` → hiding instead of overriding
- **Fix**:
```csharp
public class Animal
{
    public virtual void MakeSound() // Add virtual
    {
        Console.WriteLine("Animal sound");
    }
}

public class Dog : Animal
{
    public override void MakeSound() // Add override
    {
        Console.WriteLine("Woof!");
    }
}
```

**42. Deferred Execution** (2 pts):
- **What prints**: Nothing (no numbers > 10)
- **Why**: Query executes during foreach, sees threshold=10, filters all
- **Fix**:
```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };
var threshold = 2;
var thresholdCopy = threshold; // Capture value
var query = numbers.Where(n => n > thresholdCopy);
// OR: Force immediate execution
var query = numbers.Where(n => n > threshold).ToList();
```

**43. Generic Constraints** (2 pts):
```csharp
public class Repository<T> where T : class, IEntity, new()
{
    // class - reference type
    // IEntity - implements interface
    // new() - parameterless constructor (must be last)
}
```

**44. SOLID Violation** (2 pts):
- **Violates**: SRP (Single Responsibility Principle)
- **Reason**: Class has 4 reasons to change: data structure, database logic, email logic, validation logic
- **Fix**: Separate into User (data), UserRepository (SaveToDatabase), EmailService (SendEmail), UserValidator (ValidateEmail)

**45. Complexity** (2 pts):
1. **O(log n)** - Binary search halves search space each step
2. **O(n log n)** - QuickSort average (good pivot selection)
3. **O(n²)** - Nested loops, n × n operations
4. **O(1)** - Direct array access by index
5. **O(n)** - Must traverse linked list linearly

---

## Grading Rubric

| Section | Questions | Points Each | Max Points | Pass Requirement |
|---------|-----------|-------------|------------|------------------|
| Multiple Choice | 30 | 1 | 30 | - |
| Short Answer | 10 | 1 | 10 | - |
| Code Analysis | 5 | 2 | 10 | - |
| **Total** | **50** | - | **50** | **≥43 (85%)** |

---

## Score Interpretation

| Score Range | Result | Next Steps |
|-------------|--------|------------|
| **43-50 (85-100%)** | ✅ **PASS** - Junior Developer Certified | Proceed to Path 2 or start applying for junior roles |
| **38-42 (76-84%)** | ⚠️ **Near Pass** | Review weak areas, retake in 1 week |
| **30-37 (60-75%)** | ❌ **Not Ready** | Significant review needed, retake in 2 weeks |
| **0-29 (<60%)** | ❌ **Needs More Study** | Review all materials, practice exercises, retake in 1 month |

---

## Certification

**Upon passing (≥43 points), you earn**:

### Junior Developer Certificate
- **Path 1 Completion**: Zero to Junior Developer (6 months)
- **Skills Mastered**:
  - ✅ C# Fundamentals & OOP
  - ✅ LINQ & Functional Programming
  - ✅ Generics & Variance
  - ✅ Algorithms & Data Structures
  - ✅ Design Patterns (Builder, Observer, Decorator)
  - ✅ SOLID Principles
  - ✅ ASP.NET Core & EF Core
  - ✅ Authentication & Authorization

**Next Steps**:
1. Update resume with certification
2. Build capstone project for portfolio
3. Start Path 2 (Junior to Mid-Level) OR
4. Begin job search for junior developer positions

---

## Study Resources (Review Before Exam)

### Month 1 - C# & OOP:
- `samples/01-Beginner/PolymorphismBasics/`
- `samples/02-Intermediate/BoxingUnboxing/`
- `samples/02-Intermediate/ArrayCovariance/`

### Month 2 - LINQ:
- `samples/99-Exercises/LINQ/` (all 3 exercises)
- Deferred execution, closures, joins

### Month 3 - Generics & Patterns:
- `samples/99-Exercises/Generics/` (all 3 exercises)
- `samples/99-Exercises/DesignPatterns/01-Builder/`
- `samples/99-Exercises/DesignPatterns/02-Observer/`

### Month 4 - Algorithms:
- `samples/99-Exercises/Algorithms/` (all 3 exercises)
- Big O notation reference

### Month 5 - SOLID:
- `src/AdvancedConcepts.Core/Advanced/SOLIDPrinciples/`
- `samples/99-Exercises/DesignPatterns/03-Decorator/`

### Month 6 - Web Development:
- ASP.NET Core documentation
- EF Core relationships
- JWT authentication examples

---

## Exam Tips

1. **Read carefully**: Question wording matters ("always", "never", "can", "must")
2. **Eliminate wrong answers**: In multiple choice, remove obviously wrong options first
3. **Time management**:
   - Section 1 (MC): ~60 minutes (2 min per question)
   - Section 2 (SA): ~30 minutes (3 min per question)
   - Section 3 (Code): ~30 minutes (6 min per question)
4. **Show your work**: In code questions, partial credit given for correct thinking
5. **Use examples**: When explaining concepts, concrete examples earn more points
6. **Check answers**: Reserve last 10 minutes to review flagged questions

---

## Retake Policy

- **First retake**: Available after 1 week
- **Second retake**: Available after 2 weeks
- **Third+ retakes**: Available after 1 month
- **No limit** on number of retakes
- Study resources provided based on weak areas

---

## Good Luck! 🎯

Remember: This exam tests 6 months of learning. Take your time, think through each question carefully, and trust your preparation. You've completed all exercises, passed all monthly assessments - you're ready!

---

*Exam Version: 1.0*
*Last Updated: 2025-12-02*
*Questions: 50 | Duration: 120 min | Pass: 85%*
