using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

public class CatProducer : IProducer<Cat>
{
    public Cat Produce()
    {
        return new Cat();
    }
}