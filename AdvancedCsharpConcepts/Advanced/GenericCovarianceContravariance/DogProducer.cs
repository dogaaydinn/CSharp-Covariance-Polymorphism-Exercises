using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

public class DogProducer : IProducer<Dog>
{
    public Dog Produce()
    {
        return new Dog();
    }
}