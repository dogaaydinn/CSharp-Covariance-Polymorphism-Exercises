using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

public class CatConsumer : IConsumer<Cat>
{
    public void Consume(Cat item)
    {
        item.Speak();
    }
}