namespace AdvancedCsharpConcepts.Intermediate.CovarianceContravariance;

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

public class Animal
{
    public string Species { get; set; }

    public virtual Animal GetAnimal()
    {
        return new Animal();
    }
}

public class Cat : Animal
{
    public string Breed { get; set; }

    public override Cat GetAnimal()
    {
        return new Cat();
    }
}

public interface ICat<out T>
{
    T GetAnimal();
}

public class Cat<T> : ICat<T> where T : Animal, new()
{
    public T GetAnimal()
    {
        return new T();
    }
}