using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

public class AnimalProducer : IProducer<Animal>
{
    public Animal Produce()
    {
        return new Animal();
    }
}