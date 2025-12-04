# 🎓 Interactive Learning Exercises

**Welcome to hands-on learning!** This directory contains **interactive exercises** designed to transform passive reading into active problem-solving.

## 🎯 Philosophy

> "I hear and I forget. I see and I remember. I do and I understand." - Confucius

These exercises are **incomplete by design**. Your task is to:
1. 📖 Read the instructions
2. ✍️ Complete the missing code
3. ✅ Run tests to verify your solution
4. 🎉 Learn by doing!

---

## 📂 Exercise Categories

### 🎨 Design Patterns (`DesignPatterns/`)
Learn Gang of Four patterns through hands-on implementation:
- **Builder Pattern** - Pizza builder with fluent interface
- **Observer Pattern** - Stock ticker with IObservable/IObserver
- **Decorator Pattern** - Data source with encryption/compression

### 🧮 Algorithms (`Algorithms/`)
Master common algorithms with test-driven development:
- **Binary Search** - Iterative, recursive, and modified searches
- **QuickSort** - Partition, QuickSelect, Dutch National Flag
- **MergeSort** - Top-down, bottom-up, inversion counting

### 🔍 LINQ (`LINQ/`)
Practice LINQ queries from basic to advanced:
- **Basic Queries** - Where, Select, OrderBy, projections
- **Grouping & Aggregation** - GroupBy, Sum, Average, Count
- **Joins** - Inner joins, left joins, multiple table joins

### 🔀 Generics (`Generics/`)
Master covariance, contravariance, and generic constraints:
- **Covariance** - IEnumerable<out T>, IReadOnlyList<out T>
- **Contravariance** - IComparer<in T>, Action<in T>
- **Generic Constraints** - where T : class, struct, new(), interfaces

---

## 🚀 How to Use These Exercises

### Step 1: Choose an Exercise
```bash
cd samples/99-Exercises/DesignPatterns/StrategyPattern
```

### Step 2: Read the Instructions
```bash
cat INSTRUCTIONS.md
# or open in your favorite editor
code INSTRUCTIONS.md
```

### Step 3: Complete the Code
Look for these markers in the code:
```csharp
// TODO: Implement this method
throw new NotImplementedException();
```

### Step 4: Run the Tests
```bash
# Run tests for this exercise
dotnet test

# Expected output (before solving):
# ❌ Failed: 5 tests
# ✅ Passed: 0 tests

# Expected output (after solving):
# ✅ Passed: 5 tests
# ❌ Failed: 0 tests
```

### Step 5: Check the Solution
If you're stuck, refer to `SOLUTION.md`:
```bash
cat SOLUTION.md
```

---

## 📊 Exercise Difficulty Levels

Each exercise is marked with a difficulty level:

| Level | Icon | Description | Expected Time |
|-------|------|-------------|---------------|
| **Beginner** | 🟢 | Basic concepts, straightforward implementation | 10-15 minutes |
| **Intermediate** | 🟡 | Requires understanding of patterns/algorithms | 20-30 minutes |
| **Advanced** | 🔴 | Complex scenarios, edge cases, optimization | 45-60 minutes |
| **Expert** | 🟣 | Production-level code, performance, design | 60+ minutes |

---

## 📚 Exercise Index

### LINQ

#### 🟢 1. Basic LINQ Queries
**Path:** `LINQ/01-BasicQueries/`
**Objective:** Practice Where, Select, OrderBy, First, Count
**Concepts:** LINQ syntax, lambda expressions, query operators
**Tests:** 10 tests

#### 🟡 2. Grouping and Aggregation
**Path:** `LINQ/02-GroupingAggregation/`
**Objective:** Master GroupBy, Sum, Average, Max, Min
**Concepts:** Grouping, aggregation, anonymous types
**Tests:** 17 tests

#### 🟡 3. Joins
**Path:** `LINQ/03-Joins/`
**Objective:** Implement inner joins, left joins, and multiple table joins
**Concepts:** Join, GroupJoin, SelectMany, DefaultIfEmpty
**Tests:** 20 tests

### Algorithms

#### 🟡 4. Binary Search
**Path:** `Algorithms/01-BinarySearch/`
**Objective:** Implement iterative, recursive, and modified binary searches
**Concepts:** Divide and conquer, O(log n) complexity, edge cases
**Tests:** 26 tests

#### 🟡 5. QuickSort
**Path:** `Algorithms/02-QuickSort/`
**Objective:** Implement QuickSort, QuickSelect, and Dutch National Flag
**Concepts:** Lomuto partition, in-place sorting, 3-way partitioning
**Tests:** 30 tests

#### 🟡 6. MergeSort
**Path:** `Algorithms/03-MergeSort/`
**Objective:** Implement top-down and bottom-up MergeSort
**Concepts:** Divide and conquer, stable sorting, inversion counting
**Tests:** 22 tests

### Generics

#### 🟡 7. Covariance
**Path:** `Generics/01-Covariance/`
**Objective:** Understand and implement covariant interfaces
**Concepts:** IEnumerable<out T>, assignment compatibility
**Tests:** 12 tests

#### 🟡 8. Contravariance
**Path:** `Generics/02-Contravariance/`
**Objective:** Implement contravariant interfaces for comparers
**Concepts:** IComparer<in T>, contravariance rules
**Tests:** 10 tests

#### 🟡 9. Generic Constraints
**Path:** `Generics/03-GenericConstraints/`
**Objective:** Master generic type constraints
**Concepts:** where T : class, struct, new(), interface constraints
**Tests:** 13 tests

### Design Patterns

#### 🟡 10. Builder Pattern
**Path:** `DesignPatterns/01-Builder/`
**Objective:** Implement fluent builder for pizza construction
**Concepts:** Builder pattern, fluent interface, director pattern
**Tests:** 16 tests

#### 🟡 11. Observer Pattern
**Path:** `DesignPatterns/02-Observer/`
**Objective:** Implement stock ticker with observers
**Concepts:** IObservable<T>, IObserver<T>, event-driven architecture
**Tests:** 10 tests

#### 🟡 12. Decorator Pattern
**Path:** `DesignPatterns/03-Decorator/`
**Objective:** Implement decorators for data source
**Concepts:** Decorator pattern, component wrapping, chaining
**Tests:** 12 tests

---

## 🎯 Learning Path Recommendations

### For Beginners (New to C#)
1. Start with **Basic LINQ Queries** (🟢)
2. Move to **Binary Search** (🟡 Easy)
3. Try **Grouping and Aggregation** (🟡)
4. Complete **Builder Pattern** (🟡)

### For Intermediate Developers
1. **Covariance** (🟡)
2. **Contravariance** (🟡)
3. **Generic Constraints** (🟡)
4. **Observer Pattern** (🟡)
5. **QuickSort** (🟡)

### For Advanced Developers
1. **Joins** (🟡 Complex)
2. **MergeSort** (🟡)
3. **Decorator Pattern** (🟡)
4. Complete all exercises and optimize solutions

---

## 📈 Progress Tracking

Create a checklist to track your progress:

```markdown
## My Progress

### LINQ
- [ ] Basic Queries (🟢) - 10 tests
- [ ] Grouping & Aggregation (🟡) - 17 tests
- [ ] Joins (🟡) - 20 tests

### Algorithms
- [ ] Binary Search (🟡) - 26 tests
- [ ] QuickSort (🟡) - 30 tests
- [ ] MergeSort (🟡) - 22 tests

### Generics
- [ ] Covariance (🟡) - 12 tests
- [ ] Contravariance (🟡) - 10 tests
- [ ] Generic Constraints (🟡) - 13 tests

### Design Patterns
- [ ] Builder Pattern (🟡) - 16 tests
- [ ] Observer Pattern (🟡) - 10 tests
- [ ] Decorator Pattern (🟡) - 12 tests
```

---

## 💡 Tips for Success

### 1. Read Tests First
Tests describe the expected behavior. Start by understanding what you need to achieve:
```bash
# View test file
cat *Tests.cs
```

### 2. Use Red-Green-Refactor
1. **Red:** Run tests, see them fail
2. **Green:** Write minimal code to pass tests
3. **Refactor:** Improve code quality

### 3. Don't Peek at Solutions Too Early
Struggling is part of learning! Try for at least 15-30 minutes before checking `SOLUTION.md`.

### 4. Experiment and Break Things
- Change the code and see what breaks
- Add your own test cases
- Try different approaches

### 5. Explain Your Solution
After solving, explain your solution out loud or in comments. Teaching solidifies learning.

---

## 🧪 Testing Commands

```bash
# Run all exercises tests
cd samples/99-Exercises
dotnet test

# Run specific exercise
cd DesignPatterns/StrategyPattern
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName~CreditCardPaymentStrategy"

# Run with coverage (requires coverlet)
dotnet test /p:CollectCoverage=true
```

---

## 🏆 Completion Certificates

After completing all exercises in a category, you've mastered:

### Design Patterns Certificate
```
╔══════════════════════════════════════════════════╗
║     DESIGN PATTERNS MASTERY CERTIFICATE         ║
║                                                  ║
║  This certifies that [YOUR NAME] has            ║
║  successfully completed all Design Pattern       ║
║  exercises and demonstrated mastery of:          ║
║                                                  ║
║  ✅ Builder Pattern (Fluent Interface)          ║
║  ✅ Observer Pattern (Event-Driven)             ║
║  ✅ Decorator Pattern (Component Wrapping)      ║
║                                                  ║
║  Date: _______________                          ║
╚══════════════════════════════════════════════════╝
```

---

## 🤝 Contributing Exercises

Want to add your own exercise? Follow this structure:

```
YourExercise/
├── INSTRUCTIONS.md       # Clear objectives and requirements
├── SOLUTION.md           # Complete solution with explanations
├── YourClass.cs         # Starter code with TODOs
├── YourClassTests.cs    # Test suite (xUnit)
├── YourExercise.csproj  # Project file
└── README.md            # Overview and learning outcomes
```

**Template available at:** `_ExerciseTemplate/`

---

## 📞 Getting Help

Stuck on an exercise? Here's how to get unstuck:

1. **Re-read the instructions** - Sometimes the answer is there
2. **Check the test cases** - They describe expected behavior
3. **Google the concept** - Search "C# Strategy Pattern example"
4. **Review the main samples** - Look at `samples/02-Intermediate/` for examples
5. **Check SOLUTION.md** - As a last resort
6. **Ask the community** - Open a GitHub discussion

---

## 🎓 Learning Outcomes

After completing these exercises, you will:

✅ **Understand** design patterns deeply through implementation
✅ **Master** algorithmic thinking with TDD
✅ **Practice** LINQ queries for real-world scenarios
✅ **Comprehend** generics, covariance, and contravariance
✅ **Develop** problem-solving skills through active coding
✅ **Build** confidence in writing production-quality code

---

## 📊 Exercise Statistics

**Total Exercises:** 12 (Complete!)
**Total Tests:** 198+
**Estimated Learning Time:** 8-12 hours
**Total Code:** ~12,120 lines
**Difficulty Distribution:**
- 🟢 Beginner: 1 exercise
- 🟡 Intermediate: 11 exercises

---

## 🚀 Start Your Journey

Ready to learn by doing? Choose your first exercise:

```bash
# Start with the easiest
cd LINQ/01-BasicQueries
cat INSTRUCTIONS.md
code .

# Or jump to your interest area
cd Algorithms/01-BinarySearch
dotnet test

# Or try design patterns
cd DesignPatterns/01-Builder
dotnet test
```

**Remember:** The best way to learn coding is by writing code. Don't just read - DO!

---

**Happy Coding! 🎉**

*"The only way to learn a new programming language is by writing programs in it." - Dennis Ritchie*
