namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

public interface IConsumer<in T>
{
    void Consume(T item);
}