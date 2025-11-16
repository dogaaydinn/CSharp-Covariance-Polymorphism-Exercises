namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

/// <summary>
/// Represents a generic producer interface that demonstrates covariance.
/// The 'out' keyword makes the type parameter covariant, allowing assignment to less derived types.
/// </summary>
/// <typeparam name="T">The covariant type of items this producer creates. Can only appear in output positions.</typeparam>
/// <remarks>
/// Covariance Example: IProducer&lt;Cat&gt; can be assigned to IProducer&lt;Animal&gt;
/// because Cat derives from Animal and T is marked with 'out'.
/// </remarks>
public interface IProducer<out T>
{
    /// <summary>
    /// Produces an instance of type T.
    /// </summary>
    /// <returns>A new instance of type T.</returns>
    T Produce();
}