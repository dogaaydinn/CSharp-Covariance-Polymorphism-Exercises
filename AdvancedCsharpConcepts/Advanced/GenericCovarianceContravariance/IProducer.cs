namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

public interface IProducer<out T>
{
    T Produce();
}