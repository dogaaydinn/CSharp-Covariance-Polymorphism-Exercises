using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

/// <summary>
/// Concrete implementation of IConsumer that consumes Cat instances.
/// Demonstrates the consumer pattern with contravariance support.
/// </summary>
public class CatConsumer : IConsumer<Cat>
{
    /// <summary>
    /// Consumes a Cat instance by invoking its Speak method.
    /// </summary>
    /// <param name="item">The cat to consume.</param>
    public void Consume(Cat item)
    {
        item.Speak();
    }
}