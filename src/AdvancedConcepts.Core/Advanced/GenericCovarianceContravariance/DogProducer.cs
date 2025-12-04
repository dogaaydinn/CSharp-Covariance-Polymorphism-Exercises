using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

/// <summary>
/// Concrete implementation of IProducer that produces Dog instances.
/// Can be assigned to IProducer&lt;Animal&gt; due to covariance.
/// </summary>
/// <remarks>
/// Example: IProducer&lt;Animal&gt; producer = new DogProducer();
/// This works because IProducer&lt;T&gt; is covariant (uses 'out' keyword).
/// </remarks>
public class DogProducer : IProducer<Dog>
{
    /// <summary>
    /// Creates and returns a new Dog instance.
    /// </summary>
    /// <returns>A new Dog object.</returns>
    public Dog Produce()
    {
        return new Dog();
    }
}