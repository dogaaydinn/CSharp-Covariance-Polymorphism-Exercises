using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

/// <summary>
/// Concrete implementation of IProducer that produces Animal instances.
/// Demonstrates the producer pattern with covariance support.
/// </summary>
public class AnimalProducer : IProducer<Animal>
{
    /// <summary>
    /// Creates and returns a new Animal instance.
    /// </summary>
    /// <returns>A new Animal object.</returns>
    public Animal Produce()
    {
        return new Animal();
    }
}