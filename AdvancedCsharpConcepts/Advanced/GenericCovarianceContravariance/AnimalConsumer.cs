using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

public class AnimalConsumer : IConsumer<Animal>
{
    public void Consume(Animal item)
    {
        Console.WriteLine($"Consuming {item.GetType().Name}");
    }
}