# Month 1 Capstone: Pet Shelter Management System

**Difficulty**: ⭐⭐☆☆☆ (Beginner)
**Estimated Time**: 15-20 hours
**Prerequisites**: Completed Week 1-4 of Path 1

---

## 🎯 Project Overview

Build a complete console application that manages a pet shelter using OOP principles, polymorphism, collections, and LINQ.

### Learning Objectives

By completing this project, you will demonstrate:
- ✅ Multi-level inheritance hierarchies
- ✅ Virtual methods and polymorphism
- ✅ Generic collections (List<T>, Dictionary<TKey, TValue>)
- ✅ LINQ queries for filtering and sorting
- ✅ Avoid boxing/unboxing in collection operations
- ✅ Console UI with menu system
- ✅ Unit testing with NUnit

---

## 📋 Requirements

### Functional Requirements

1. **Animal Hierarchy** (3+ levels):
   ```csharp
   Animal (base)
   ├── Mammal
   │   ├── Dog
   │   └── Cat
   ├── Bird
   │   ├── Parrot
   │   └── Canary
   └── Reptile
       └── Turtle
   ```

2. **Animal Properties**:
   - Name (string)
   - Age (int)
   - Species (string)
   - Status (Available, Adopted, UnderCare)
   - AdoptionDate (DateTime?)

3. **Polymorphic Behaviors** (minimum 5 virtual methods):
   - `MakeSound()` - Each animal makes different sound
   - `Eat()` - Different eating behaviors
   - `GetCareInstructions()` - Species-specific care
   - `GetDescription()` - Full animal description
   - `CalculateDailyCost()` - Care cost varies by species

4. **Shelter Operations**:
   - Add new animal to shelter
   - Remove animal (adoption or transfer)
   - List all animals
   - List available animals
   - List adopted animals
   - Search animals by name
   - Search animals by species
   - Filter animals by age range
   - Sort animals by name, age, or arrival date

5. **Statistics**:
   - Total animals in shelter
   - Animals by species count
   - Average age of animals
   - Total daily care cost
   - Adoption rate (adopted / total)

6. **Console Menu System**:
   ```
   === PET SHELTER MANAGEMENT SYSTEM ===
   1. Add Animal
   2. List All Animals
   3. Search Animal
   4. Adopt Animal
   5. View Statistics
   6. Exit

   Enter your choice:
   ```

### Technical Requirements

1. **No Boxing**: All collections must use generics (List<T>, not ArrayList)
2. **LINQ Usage**: Minimum 10 different LINQ queries
3. **Unit Tests**: Minimum 10 tests covering core functionality
4. **Error Handling**: Graceful handling of invalid input
5. **Code Organization**: Separate files for models, services, UI
6. **Documentation**: XML comments on public methods

---

## 🏗️ Project Structure

```
PetShelterSystem/
├── PetShelterSystem.csproj
├── Program.cs                      // Entry point
├── Models/
│   ├── Animal.cs                   // Base abstract class
│   ├── Mammal.cs                   // Intermediate class
│   ├── Dog.cs                      // Concrete implementation
│   ├── Cat.cs
│   ├── Bird.cs
│   ├── Parrot.cs
│   ├── Canary.cs
│   ├── Reptile.cs
│   ├── Turtle.cs
│   └── AdoptionStatus.cs           // Enum
├── Services/
│   ├── ShelterService.cs           // Core business logic
│   └── StatisticsService.cs        // Statistics calculations
├── UI/
│   └── ConsoleUI.cs                // Menu and user interaction
└── PetShelterSystem.Tests/
    ├── PetShelterSystem.Tests.csproj
    └── ShelterServiceTests.cs
```

---

## 🚀 Getting Started

### Step 1: Create the Project

```bash
# Create solution and projects
dotnet new console -n PetShelterSystem
cd PetShelterSystem
dotnet new nunit -n PetShelterSystem.Tests
dotnet add PetShelterSystem.Tests reference PetShelterSystem.csproj
dotnet new sln -n PetShelterSystem
dotnet sln add PetShelterSystem.csproj PetShelterSystem.Tests/PetShelterSystem.Tests.csproj
```

### Step 2: Implement Animal Hierarchy

Start with the base `Animal` class:

```csharp
// Models/Animal.cs
namespace PetShelterSystem.Models;

public abstract class Animal
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Species { get; set; }
    public AdoptionStatus Status { get; set; }
    public DateTime ArrivalDate { get; set; }
    public DateTime? AdoptionDate { get; set; }

    protected Animal(string name, int age, string species)
    {
        Name = name;
        Age = age;
        Species = species;
        Status = AdoptionStatus.Available;
        ArrivalDate = DateTime.Now;
    }

    // TODO: Implement abstract method MakeSound()
    public abstract string MakeSound();

    // TODO: Implement virtual method Eat()
    public virtual string Eat()
    {
        return $"{Name} is eating.";
    }

    // TODO: Implement virtual method GetCareInstructions()
    public virtual string GetCareInstructions()
    {
        return $"Standard care for {Species}.";
    }

    // TODO: Implement virtual method GetDescription()
    public virtual string GetDescription()
    {
        return $"{Name} - {Species}, Age: {Age}, Status: {Status}";
    }

    // TODO: Implement virtual method CalculateDailyCost()
    public virtual decimal CalculateDailyCost()
    {
        return 10.00m; // Base cost
    }
}
```

```csharp
// Models/AdoptionStatus.cs
namespace PetShelterSystem.Models;

public enum AdoptionStatus
{
    Available,
    Adopted,
    UnderCare
}
```

### Step 3: Implement Derived Classes

Example for Dog:

```csharp
// Models/Mammal.cs
namespace PetShelterSystem.Models;

public abstract class Mammal : Animal
{
    public bool IsFurry { get; set; }

    protected Mammal(string name, int age, string species)
        : base(name, age, species)
    {
        IsFurry = true;
    }

    public override string Eat()
    {
        return $"{Name} is chewing food.";
    }
}
```

```csharp
// Models/Dog.cs
namespace PetShelterSystem.Models;

public class Dog : Mammal
{
    public string Breed { get; set; }

    public Dog(string name, int age, string breed)
        : base(name, age, "Dog")
    {
        Breed = breed;
    }

    // TODO: Override MakeSound() - return "Woof!"
    public override string MakeSound()
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    // TODO: Override GetCareInstructions()
    public override string GetCareInstructions()
    {
        // TODO: Implement dog-specific care
        throw new NotImplementedException();
    }

    // TODO: Override CalculateDailyCost()
    public override decimal CalculateDailyCost()
    {
        // TODO: Dogs cost more (food, grooming, etc.)
        throw new NotImplementedException();
    }
}
```

**YOUR TASK**: Implement Cat, Bird, Parrot, Canary, Reptile, Turtle classes following the same pattern.

### Step 4: Implement ShelterService

```csharp
// Services/ShelterService.cs
using PetShelterSystem.Models;

namespace PetShelterSystem.Services;

public class ShelterService
{
    private readonly List<Animal> _animals;

    public ShelterService()
    {
        _animals = new List<Animal>();
    }

    // TODO: Implement AddAnimal(Animal animal)
    public void AddAnimal(Animal animal)
    {
        // TODO: Add animal to collection
        throw new NotImplementedException();
    }

    // TODO: Implement RemoveAnimal(string name)
    public bool RemoveAnimal(string name)
    {
        // TODO: Find and remove animal by name
        throw new NotImplementedException();
    }

    // TODO: Implement GetAllAnimals() - returns IEnumerable<Animal>
    public IEnumerable<Animal> GetAllAnimals()
    {
        // TODO: Return all animals using LINQ
        throw new NotImplementedException();
    }

    // TODO: Implement GetAvailableAnimals()
    public IEnumerable<Animal> GetAvailableAnimals()
    {
        // TODO: Filter animals where Status == Available
        throw new NotImplementedException();
    }

    // TODO: Implement SearchByName(string name)
    public IEnumerable<Animal> SearchByName(string name)
    {
        // TODO: Use LINQ to search (case-insensitive)
        throw new NotImplementedException();
    }

    // TODO: Implement SearchBySpecies(string species)
    public IEnumerable<Animal> SearchBySpecies(string species)
    {
        // TODO: Use LINQ to filter by species
        throw new NotImplementedException();
    }

    // TODO: Implement FilterByAgeRange(int minAge, int maxAge)
    public IEnumerable<Animal> FilterByAgeRange(int minAge, int maxAge)
    {
        // TODO: Use LINQ Where clause
        throw new NotImplementedException();
    }

    // TODO: Implement AdoptAnimal(string name)
    public bool AdoptAnimal(string name)
    {
        // TODO: Find animal, set Status = Adopted, set AdoptionDate
        throw new NotImplementedException();
    }

    // TODO: Implement GetTotalCount()
    public int GetTotalCount()
    {
        // TODO: Return count of all animals
        throw new NotImplementedException();
    }

    // TODO: Implement GetAverageAge()
    public double GetAverageAge()
    {
        // TODO: Use LINQ Average()
        throw new NotImplementedException();
    }

    // TODO: Implement GetTotalDailyCost()
    public decimal GetTotalDailyCost()
    {
        // TODO: Use LINQ Sum() on CalculateDailyCost()
        throw new NotImplementedException();
    }

    // TODO: Implement GetSpeciesCount()
    public Dictionary<string, int> GetSpeciesCount()
    {
        // TODO: Use LINQ GroupBy() and ToDictionary()
        throw new NotImplementedException();
    }
}
```

### Step 5: Implement Console UI

```csharp
// UI/ConsoleUI.cs
using PetShelterSystem.Models;
using PetShelterSystem.Services;

namespace PetShelterSystem.UI;

public class ConsoleUI
{
    private readonly ShelterService _service;

    public ConsoleUI()
    {
        _service = new ShelterService();
    }

    public void Run()
    {
        // TODO: Add some sample animals for testing
        SeedData();

        while (true)
        {
            ShowMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddAnimal();
                    break;
                case "2":
                    ListAllAnimals();
                    break;
                case "3":
                    SearchAnimal();
                    break;
                case "4":
                    AdoptAnimal();
                    break;
                case "5":
                    ShowStatistics();
                    break;
                case "6":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine("=== PET SHELTER MANAGEMENT SYSTEM ===");
        Console.WriteLine("1. Add Animal");
        Console.WriteLine("2. List All Animals");
        Console.WriteLine("3. Search Animal");
        Console.WriteLine("4. Adopt Animal");
        Console.WriteLine("5. View Statistics");
        Console.WriteLine("6. Exit");
        Console.WriteLine();
        Console.Write("Enter your choice: ");
    }

    // TODO: Implement all menu methods
    private void AddAnimal() { /* TODO */ }
    private void ListAllAnimals() { /* TODO */ }
    private void SearchAnimal() { /* TODO */ }
    private void AdoptAnimal() { /* TODO */ }
    private void ShowStatistics() { /* TODO */ }

    private void SeedData()
    {
        // Add sample animals
        _service.AddAnimal(new Dog("Buddy", 3, "Golden Retriever"));
        _service.AddAnimal(new Dog("Max", 5, "German Shepherd"));
        // TODO: Add more sample animals
    }
}
```

### Step 6: Write Unit Tests

```csharp
// PetShelterSystem.Tests/ShelterServiceTests.cs
using NUnit.Framework;
using PetShelterSystem.Models;
using PetShelterSystem.Services;

namespace PetShelterSystem.Tests;

[TestFixture]
public class ShelterServiceTests
{
    private ShelterService _service;

    [SetUp]
    public void Setup()
    {
        _service = new ShelterService();
    }

    [Test]
    public void AddAnimal_ShouldIncreaseCount()
    {
        // Arrange
        var dog = new Dog("Buddy", 3, "Golden Retriever");

        // Act
        _service.AddAnimal(dog);

        // Assert
        Assert.That(_service.GetTotalCount(), Is.EqualTo(1));
    }

    [Test]
    public void GetAvailableAnimals_ShouldReturnOnlyAvailable()
    {
        // TODO: Implement test
        Assert.Fail("Test not implemented");
    }

    [Test]
    public void AdoptAnimal_ShouldChangeStatus()
    {
        // TODO: Implement test
        Assert.Fail("Test not implemented");
    }

    // TODO: Implement at least 7 more tests
}
```

---

## 🎯 Milestones

### Milestone 1: Basic Structure (Day 1-2)
- ✅ Project setup complete
- ✅ Animal hierarchy defined (all 7+ classes)
- ✅ All abstract/virtual methods implemented
- ✅ Polymorphism working (each animal makes unique sound)

### Milestone 2: Core Functionality (Day 3-4)
- ✅ ShelterService fully implemented
- ✅ All LINQ queries working
- ✅ Add/remove animals functional
- ✅ Search and filter working

### Milestone 3: UI and Polish (Day 5-6)
- ✅ Console menu system complete
- ✅ All menu options functional
- ✅ Input validation and error handling
- ✅ Statistics display working

### Milestone 4: Testing and Documentation (Day 7)
- ✅ 10+ unit tests passing
- ✅ XML documentation comments added
- ✅ README with usage instructions
- ✅ Code review checklist completed

---

## ✅ Evaluation Criteria

Your project will be evaluated on:

| Criteria | Points | Requirements |
|----------|--------|--------------|
| **Functionality** | 40 | All required features working |
| **Code Quality** | 30 | Clean code, proper OOP, SOLID principles |
| **Tests** | 20 | 10+ tests, good coverage |
| **Documentation** | 10 | XML comments, README |
| **TOTAL** | **100** | **Pass: 75+** |

### Detailed Rubric

**Functionality (40 points)**:
- Animal hierarchy (3+ levels): 10 pts
- Polymorphic behaviors (5+ methods): 10 pts
- LINQ queries (10+ different): 10 pts
- Console UI complete: 10 pts

**Code Quality (30 points)**:
- No boxing in collections: 5 pts
- Proper use of generics: 5 pts
- Clean code (naming, organization): 10 pts
- Error handling: 5 pts
- Separation of concerns: 5 pts

**Tests (20 points)**:
- 10+ tests: 10 pts
- Tests pass: 5 pts
- Good coverage: 5 pts

**Documentation (10 points)**:
- XML comments: 5 pts
- README: 5 pts

---

## 💡 Tips

1. **Start Simple**: Implement one animal type completely before adding more
2. **Test Early**: Write tests as you build, not at the end
3. **Use LINQ**: Practice different LINQ queries (Where, Select, GroupBy, OrderBy, etc.)
4. **Polymorphism**: Make sure each animal has unique behavior
5. **Avoid Duplication**: Use inheritance to share common code
6. **Console UI**: Keep it simple - focus on functionality over fancy formatting
7. **Error Handling**: Handle null inputs, invalid ages, etc.
8. **XML Comments**: Use `///` comments for all public methods

---

## 🚀 Extensions (Optional)

If you finish early, add these features:

1. **Save/Load**: Persist data to JSON file
2. **Advanced Search**: Search by multiple criteria
3. **Adoption History**: Track previous adoptions
4. **Medical Records**: Add vaccination, checkup dates
5. **Staff Management**: Track employees and assignments
6. **Reports**: Generate daily/weekly reports
7. **Unit Tests**: Increase to 20+ tests
8. **Performance**: Benchmark LINQ queries with BenchmarkDotNet

---

## 📚 Resources

- **Reference Material**:
  - `samples/01-Beginner/PolymorphismBasics/`
  - `samples/01-Beginner/AssignmentCompatibility/`
  - `samples/99-Exercises/LINQ/01-BasicQueries/`

- **LINQ Documentation**:
  - https://learn.microsoft.com/en-us/dotnet/csharp/linq/

- **NUnit Documentation**:
  - https://docs.nunit.org/

---

## 🎉 Submission

When complete, your submission should include:

1. Complete source code (all classes implemented)
2. All tests passing (10+)
3. README.md with:
   - Project description
   - How to run
   - Features implemented
   - Known limitations
4. Screenshot of running application
5. Test results screenshot

---

**Good luck with your capstone project!** 🚀

Remember: This project demonstrates everything you learned in Month 1. Take your time, write clean code, and make it something you're proud to show in your portfolio!

---

*Template Version: 1.0*
*Last Updated: 2025-12-02*
