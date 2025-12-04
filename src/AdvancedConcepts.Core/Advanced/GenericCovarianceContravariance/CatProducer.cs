using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

/// <summary>
/// Concrete implementation of IProducer that produces Cat instances.
/// Can be assigned to IProducer&lt;Animal&gt; due to covariance.
/// </summary>
/// <remarks>
/// Example: IProducer&lt;Animal&gt; producer = new CatProducer();
/// This works because IProducer&lt;T&gt; is covariant (uses 'out' keyword).
/// </remarks>
public class CatProducer : IProducer<Cat>
{
    /// <summary>
    /// Creates and returns a new Cat instance.
    /// </summary>
    /// <returns>A new Cat object.</returns>
    public Cat Produce()
    {
        return new Cat();
    }
}