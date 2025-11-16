using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

/// <summary>
/// Concrete implementation of IConsumer that consumes Animal instances.
/// Can be assigned to IConsumer&lt;Cat&gt; or IConsumer&lt;Dog&gt; due to contravariance.
/// </summary>
/// <remarks>
/// Example: IConsumer&lt;Cat&gt; consumer = new AnimalConsumer();
/// This works because IConsumer&lt;T&gt; is contravariant (uses 'in' keyword).
/// A consumer that can handle any Animal can safely handle specific animal types.
/// </remarks>
public class AnimalConsumer : IConsumer<Animal>
{
    /// <summary>
    /// Consumes an Animal instance and displays its type name.
    /// </summary>
    /// <param name="item">The animal to consume.</param>
    public void Consume(Animal item)
    {
        Console.WriteLine($"Consuming {item.GetType().Name}");
    }
}