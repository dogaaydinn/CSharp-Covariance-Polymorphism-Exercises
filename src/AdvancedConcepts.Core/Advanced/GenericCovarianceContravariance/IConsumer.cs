namespace AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;

/// <summary>
/// Represents a generic consumer interface that demonstrates contravariance.
/// The 'in' keyword makes the type parameter contravariant, allowing assignment from less derived types.
/// </summary>
/// <typeparam name="T">The contravariant type of items this consumer accepts. Can only appear in input positions.</typeparam>
/// <remarks>
/// Contravariance Example: IConsumer&lt;Animal&gt; can be assigned to IConsumer&lt;Cat&gt;
/// because a consumer that handles Animal can safely handle any Cat (since Cat is an Animal).
/// </remarks>
public interface IConsumer<in T>
{
    /// <summary>
    /// Consumes an item of type T.
    /// </summary>
    /// <param name="item">The item to consume.</param>
    void Consume(T item);
}