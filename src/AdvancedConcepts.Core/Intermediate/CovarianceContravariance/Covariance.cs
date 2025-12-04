namespace AdvancedCsharpConcepts.Intermediate.CovarianceContravariance;

/// <summary>
/// Demonstrates covariance concepts in C# across different contexts:
/// arrays, delegates, generic types, and return types.
/// </summary>
/// <remarks>
/// Covariance allows using a more derived type than originally specified.
/// It applies to:
/// - Array types (e.g., Cat[] can be assigned to Animal[])
/// - Delegate return types (using 'out' keyword)
/// - Generic type parameters (using 'out' keyword for interfaces/delegates)
/// - Method return types (covariant return types in C# 9+)
/// </remarks>
public class Covariance
{
    // out keywordünü sadece delegate ve generic tür parametrelerinde kullanabiliriz.
    private static XHandler<Animal> animalHandler = GetAnimal;
    private static XHandler<Cat> catHandler = GetCat;

    private Animal animal = new Cat();

    #region GenericTypes

    //generic tür parametresini oluşturmak istediğimiz, ilgili tür parametresini out keywordü ile işaretlememiz gerekmektedir.
    private ICat<Animal> animal1 = new Cat<Animal>();

    #endregion

    private Cat cat = new();

    #region ArrayTypes

    /// <summary>
    /// Demonstrates covariance with arrays and IEnumerable&lt;T&gt; collections.
    /// Shows how derived types can be assigned to base type collections.
    /// </summary>
    /// <remarks>
    /// Warning: Array covariance can lead to runtime errors (ArrayTypeMismatchException)
    /// if you try to add incompatible types. Generic collections with 'out' keyword are safer.
    /// </remarks>
    public static void DemonstrateCovariance()
    {
        // Covariance example
        Animal[] animals = new Cat[5]; // covariance

        // Covariance with object array
        object[] objects = new string[5]; // covariance
        objects[0] = "123"; // this is fine
        // objects[1] = 123; // this will cause a runtime error

        IEnumerable<Animal> animals1 = new List<Cat>(); // covariance
        IEnumerable<Animal> animals2 = new Cat[5]; // covariance

        // Using animals2
        foreach (var animal in animals2) Console.WriteLine(animal?.Species);
    }

    #endregion

    private static Animal GetAnimal()
    {
        return new Animal();
    }

    private delegate T XHandler<out T>();

    #region DelegateTypes

    //delegateler sadece eşleştikleri imzalara sahip olan metotlara değil, aynı zamanda delegatein kullandığı return typedan türemiş olan türlere de eşleşebilirler.

    private Func<Animal> getAnimal = () => GetCat();

    private static Cat GetCat()
    {
        return new Cat();
    }

    #endregion

    #region ReturnTypes

    //covariance ovveride edilen metotlarda kullanılır, metodu override ederken return type ı base classta kullanılan türden türemiş bir tür olarak tanımlamamızı sağlar.
    //C# 9 ve sonrasında covariant return types özelliği eklenmiştir.

    #endregion
}

/// <summary>
/// Represents a base animal class used for demonstrating covariance.
/// </summary>
public class Animal
{
    /// <summary>
    /// Gets or sets the species name of the animal.
    /// </summary>
    public string Species { get; set; } = string.Empty;

    /// <summary>
    /// Creates and returns a new Animal instance.
    /// This virtual method demonstrates covariant return types when overridden.
    /// </summary>
    /// <returns>A new Animal instance.</returns>
    public virtual Animal GetAnimal()
    {
        return new Animal();
    }
}

/// <summary>
/// Represents a cat, which is a specific type of animal.
/// Demonstrates covariant return types in method overriding (C# 9+).
/// </summary>
public class Cat : Animal
{
    /// <summary>
    /// Gets or sets the breed of the cat.
    /// </summary>
    public string Breed { get; set; } = string.Empty;

    /// <summary>
    /// Creates and returns a new Cat instance.
    /// Demonstrates covariant return type - returns Cat instead of Animal.
    /// </summary>
    /// <returns>A new Cat instance.</returns>
    public override Cat GetAnimal()
    {
        return new Cat();
    }
}

/// <summary>
/// Generic interface demonstrating covariance with the 'out' keyword.
/// The type parameter T is covariant, allowing derived type assignments.
/// </summary>
/// <typeparam name="T">The covariant type parameter (can only be used in output positions).</typeparam>
public interface ICat<out T>
{
    /// <summary>
    /// Produces an instance of type T.
    /// </summary>
    /// <returns>An instance of type T.</returns>
    T GetAnimal();
}

/// <summary>
/// Generic cat class implementing the covariant ICat&lt;T&gt; interface.
/// Demonstrates how covariance works with generic type parameters.
/// </summary>
/// <typeparam name="T">Animal type that must have a parameterless constructor.</typeparam>
public class Cat<T> : ICat<T> where T : Animal, new()
{
    /// <summary>
    /// Creates and returns a new instance of type T.
    /// </summary>
    /// <returns>A new instance of type T.</returns>
    public T GetAnimal()
    {
        return new T();
    }
}