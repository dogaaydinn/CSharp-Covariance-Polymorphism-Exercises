using AdvancedCsharpConcepts.Advanced.ExplicitImplicitConversion;
using AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;
using AdvancedCsharpConcepts.Beginner.Override_Upcast_Downcast;
using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;
using AdvancedCsharpConcepts.Beginner.Upcast_Downcast;
using AdvancedCsharpConcepts.Intermediate.BoxingUnboxing;
using AdvancedCsharpConcepts.Intermediate.CovarianceContravariance;
using AdvancedCsharpConcepts.Beginner;
using AdvancedCsharpConcepts.Advanced.ModernCSharp;
using AdvancedCsharpConcepts.Advanced.HighPerformance;
using AdvancedCsharpConcepts.Advanced.PerformanceBenchmarks;
using Animal = AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility.Animal;
using Cat = AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility.Cat;
using Mammal = AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility.Mammal;

namespace AdvancedCsharpConcepts;

using Animal = Beginner.Polymorphism_AssignCompatibility.Animal;
using Cat = Beginner.Polymorphism_AssignCompatibility.Cat;

internal static class Program
{
    private static void Main(string[] args)
    {
        // Parse command-line arguments
        var runBenchmarks = args.Contains("--benchmark") || args.Contains("-b");
        var runAdvancedOnly = args.Contains("--advanced") || args.Contains("-a");
        var runBasicsOnly = args.Contains("--basics") || args.Contains("--basic");
        var showHelp = args.Contains("--help") || args.Contains("-h");

        if (showHelp)
        {
            ShowHelp();
            return;
        }

        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   Advanced C# Concepts - From Basics to High Performance     ║");
        Console.WriteLine("║   Silicon Valley × NVIDIA Developer Best Practices            ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        if (runBenchmarks)
        {
            BenchmarkRunner.RunAllBenchmarks();
            return;
        }

        if (runAdvancedOnly)
        {
            RunAdvancedExamples();
            return;
        }

        if (runBasicsOnly)
        {
            RunBasicExamples();
            return;
        }

        // Default: run everything
        RunBasicExamples();
        RunAdvancedExamples();
        BenchmarkRunner.ShowQuickSummary();
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Advanced C# Concepts - Command Line Options\n");
        Console.WriteLine("Usage: dotnet run [options]\n");
        Console.WriteLine("Options:");
        Console.WriteLine("  --help, -h         Show this help message");
        Console.WriteLine("  --basics, --basic  Run only basic examples (polymorphism, casting, etc.)");
        Console.WriteLine("  --advanced, -a     Run only advanced examples (C# 12, Span<T>, parallelism)");
        Console.WriteLine("  --benchmark, -b    Run performance benchmarks (requires Release mode)");
        Console.WriteLine("\nExamples:");
        Console.WriteLine("  dotnet run                          # Run all examples");
        Console.WriteLine("  dotnet run --basics                 # Run basic examples only");
        Console.WriteLine("  dotnet run --advanced               # Run advanced examples only");
        Console.WriteLine("  dotnet run -c Release --benchmark   # Run benchmarks\n");
    }

    private static void RunBasicExamples()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("                    BASIC EXAMPLES                              ");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        #region Polymorphism-AssignCompatibility- Assignment Compatibility
        //Soru 1: Bir Vehicle sınıfı oluştur ve bunu kalıtım yoluyla Car ve Bike sınıflarına genişlet. Bir List<Vehicle> içerisine hem Car hem de Bike ekle ve her bir nesnenin override edilmiş metotlarını çağır.
        //Question 1: Create a Vehicle class and extend it by inheritance to Car and Bike. Add both Car and Bike to a List<Vehicle> and call the overridden methods of each object.
        Console.WriteLine("\nPolymorphism- Assignment Compatibility:");
        Console.WriteLine("Check debug console for the output.\n");
        Console.WriteLine("If you want to see the output, please uncomment the related code block.\n");
        Mammal mammal = new Animal();
        var animal = (Animal)mammal;
        var mammal1 = mammal; // yeni nesne oluşturulmadan referans oluşturuldu.- reference created without creating a new object.
        //bir nesneyi kendisi ve üstündeki sınıfların referansları işaretleyebilir
        //an object can point to itself and references to the classes above it
        //  Cat cat = (Cat)mammal1; // ifadesi, mammal1 nesnesini Cat türüne dönüştürmeye çalışır. Bu işlem, mammal1 nesnesinin aslında Cat türünde bir nesne olduğunu varsayar.değilse, çalışma zamanında bir InvalidCastException hatası oluşur.
        // Cat cat = (Cat)mammal1; // attempts to cast the mammal1 object to the Cat type. This assumes that mammal1 is actually an object of type Cat. If it is not, an InvalidCastException error occurs at runtime.
        var cat = mammal1 as Cat; // as kullandığımızdan run time sırasında hata verek yerine null verdi- because we used as, it gave null instead of giving an error during run time
        Animal animal1 = new Cat();
        var cat1 = (Cat)animal1;
        var dog = new Dog();
        Animal animal2 = dog;
        var dog1 = new Dog();
        Mammal mammal2 = dog1;
        var dog2 = (Dog)mammal2;
        Console.WriteLine("Assignment Compatibility completed.\n");

        #endregion

        #region is operator

        Console.WriteLine("is operator:");

        Mammal mammal3 = new Dog { Species = "Dog", Breed = "Golden Retriever", Name = "Max" };
        Console.WriteLine($"Mammal is dog: {mammal3 is Dog}"); // true
        Console.WriteLine($"Mammal is mammal: {true}"); // true
        Console.WriteLine($"Mammal is animal: {mammal3 is Animal}"); // true
        Console.WriteLine($"Mammal is cat: {mammal3 is Cat}"); // false
        Console.WriteLine("is operator completed.\n");

        #endregion

        #region Polymorphism-AssignCompatibility- Method Overriding

        Console.WriteLine("Polymorphism-AssignCompatibility- Method Overriding:");

        // Soru 1: Bir Vehicle sınıfı oluştur ve bunu kalıtım yoluyla Car ve Bike sınıflarına genişlet. Bir List<Vehicle> içerisine hem Car hem de Bike ekle ve her bir nesnenin override edilmiş metotlarını çağır.
        // Question 1: Create a Vehicle class and extend it by inheritance to Car and Bike. Add both Car and Bike to a List<Vehicle> and call the overridden methods of each object.
        Console.WriteLine("Creating a Vehicle class and extending it by inheritance to Car and Bike:");
        Console.WriteLine("Check car and bike class for more understanding.\n");
        var vehicles = new List<Vehicle>
        {
            new Car(),
            new Bike()
        };

        foreach (var vehicle in vehicles) vehicle.Drive();
        Console.WriteLine("Method Overriding completed.\n");

        #endregion

        #region Upcasting-Downcasting
         //Soru 2: Car sınıfındaki bir nesneyi Vehicle türüne cast et ve ardından tekrar Car türüne geri cast et.
         //Question 2: Cast an object of class Car to type Vehicle and then cast it back to type Car.
         Console.WriteLine("Upcasting-Downcasting:");
Console.WriteLine("Cast an object of class Car to type Vehicle and then cast it back to type Car.");

        Console.WriteLine("Upcasting:");
        Console.WriteLine("Check car class and CastExample method for understanding.\n");
        Car.CastExample();
        Console.WriteLine("Upcasting-Downcasting completed.\n");

        #endregion

        #region Assignment Compatibility

        Console.WriteLine("Assignment Compatibility:");
        //Soru 1: Aşağıdaki kod çalıştırıldığında ne olur? object ve string türleri arasındaki assignment compatibility’yi açıklayın:
        // Question 1: What happens when the following code is executed? Explain the assignment compatibility between object and string types:
        Console.WriteLine("Check debug console for the output.\n");
        var assignmentCompatibility = new AssignmentCompatibility();
        assignmentCompatibility.TypeChecking();

        Console.WriteLine("Assignment Compatibility completed.\n");

        #endregion

        #region Downcasting

        Console.WriteLine("Downcasting:");
        //Soru 2: Bir Employee sınıfı ile kalıtım yoluyla Manager sınıfını oluştur. Manager nesnesini Employee türüne atayıp, Manager’a geri çevirin (downcast). Bu işlemin başarı durumunu kontrol edin.
        //Question 2: Create the Manager class by inheritance from an Employee class. Assign the Manager object to the Employee type and downcast it back to Manager. Check the success of this operation.
        Console.WriteLine("Creating an Employee class and extending it by inheritance to Manager:");

        Manager.DownCast();
        Console.WriteLine("Downcasting completed.\n");

        #endregion

        #region Covariance

        Console.WriteLine("Covariance:");
        //Soru 1: IEnumerable<string> türünde bir liste oluştur ve bunu IEnumerable<object> türüne atamaya çalışın. Bu işlemin nasıl başarılı olduğunu açıklayın.
        // Question 1: Create a list of type IEnumerable<string> and try to assign it to type IEnumerable<object>. Explain how this succeeds.
        Console.WriteLine("Create a list of type IEnumerable<string> and try to assign it to type IEnumerable<object>. Explain how this succeeds.");
        var covarianceContravariance = new VarianceExamples();

        Console.WriteLine("Contravariance:");
        covarianceContravariance.CovarianceExample();
        //Soru 2: Action<object> türünde bir delege oluşturun ve bunu Action<string> ile atamayı deneyin. Neden derleme hatası alındığını açıklayın.
        //Question 2: Create a delegate of type Action<object> and try to assign it with Action<string>. Explain why a compilation error is received.
        Console.WriteLine("Create a delegate of type Action<object> and try to assign it to Action<string>. Explain why a compilation error is received.");
        covarianceContravariance.ContravarianceExample();
        Console.WriteLine("Covariance completed.\n");

        #endregion

        #region Boxing ve Unboxing:

        Console.WriteLine("Boxing ve Unboxing:");
        //Soru 1: Bir int değişkeni boxing ile object türüne dönüştürün. Daha sonra bunu tekrar int türüne unboxing yapın. Bu işlemler sırasında hafıza yönetimi açısından neler olduğunu açıklayın.
        //Question 1: Convert an int variable to object with boxing. Then unboxing it back to int type. Explain what happens during these operations in terms of memory management.
        Console.WriteLine("Convert an int variable to object type with boxing. Then unboxing it back to int type. Explain what happens during these operations in terms of memory management.");
        BoxingUnboxingExamples.BoxingUnboxingExample();

        // Question 2: Using ArrayList, add data of different types (e.g. int and string). Then retrieve this data with unboxing and analyze the performance differences.
        //Soru 2: ArrayList kullanarak farklı türlerden veriler ekleyin (örneğin int ve string). Ardından bu verileri unboxing ile geri alın ve performans farklarını analiz edin.
        Console.WriteLine("ArrayList: Add data of different types (e.g. int and string) using ArrayList. Then retrieve this data with unboxing and analyze the performance differences.");
        BoxingUnboxingExamples.ArrayListExample();
        Console.WriteLine("Boxing ve Unboxing completed.\n");

        #endregion

        #region Explicit ve Implicit Conversion:

        Console.WriteLine("Explicit ve Implicit Conversion:");
        Console.WriteLine("Temperature conversion:");
        Console.WriteLine("Check debug console for the output. And temperature class for more understanding. \n");
        var tempC = new Temperature(25); // 25 °C

        // Implicit conversion from Celsius to Fahrenheit
        TemperatureFahrenheit tempF = tempC; // °F conversion
        Console.WriteLine($"{tempC} = {tempF}"); // 25 °C = 77 °F

        // Implicit conversion from Fahrenheit to Celsius
        TemperatureCelsius convertedTempC = tempF; // °C conversion
        Console.WriteLine($"{tempF} = {convertedTempC}"); // 77 °F = 25 °C

        // Soru 2: Implicit ve explicit conversion örnekleri
        var explicitImplicitConversion = new ConversionExamples();
        explicitImplicitConversion.ConversionExample();
        Console.WriteLine("Explicit ve Implicit Conversion completed.\n");

        #endregion

        #region Generic Covariance ve Contravariance

        //Soru 1: IProducer<Animal> ve IConsumer<Animal> gibi iki generic interface tanımlayın. Covariance ve contravariance ile bunlar arasında dönüşüm yapmayı deneyin.
        //Question 1: Define two generic interfaces like IProducer<Animal> and IConsumer<Animal>. Try to transform between them with covariance and contravariance.
        // Covariance (Out) Example
        Console.WriteLine("Generic Covariance ve Contravariance:");

        IProducer<Animal> animalProducer = new CatProducer(); // You can use CatProducer as IProducer<Animal>.
        var myAnimal = animalProducer.Produce(); // Covariant
        myAnimal.Speak(); // Output: Cat meows

        // Contravariance (In) Example
        IConsumer<Cat> catConsumer = new AnimalConsumer(); // You can use AnimalConsumer as IConsumer<Cat>.
        catConsumer.Consume(new Cat()); // Contravariant

        //Soru 2: Aşağıdaki kodu inceleyin ve generics ile covariance'ın nasıl çalıştığını açıklayın:
        // Question 2: Analyze the following code and explain how generics and covariance work:
        // Func<Animal> animalProducer = () => new Dog();
        // Func<Dog> dogProducer = animalProducer; // Covariance

        IProducer<Dog> dogProducer = new DogProducer();
        animalProducer = dogProducer;

        myAnimal = animalProducer.Produce();
        myAnimal.Speak(); // Output: "Dog barks"
        Console.WriteLine("Generic Covariance ve Contravariance completed.\n");

        #endregion
    }

    private static void RunAdvancedExamples()
    {
        Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
        Console.WriteLine("                    ADVANCED EXAMPLES                           ");
        Console.WriteLine("         Silicon Valley × NVIDIA Best Practices                 ");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        #region C# 12 Primary Constructors

        PrimaryConstructorsExample.RunExample();

        #endregion

        #region C# 12 Collection Expressions

        CollectionExpressionsExample.RunExample();

        #endregion

        #region Advanced Pattern Matching

        AdvancedPatternMatching.RunExample();

        #endregion

        #region High-Performance Parallel Processing

        ParallelProcessingExamples.RunExample();

        #endregion

        #region Span<T> and Memory<T> - Zero Allocation

        SpanMemoryExamples.RunExample();

        #endregion

        Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
        Console.WriteLine("                 ALL EXAMPLES COMPLETED                         ");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");
    }
}
