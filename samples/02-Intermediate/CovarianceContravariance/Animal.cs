namespace CovarianceContravariance;

public class Animal
{
    public string Name { get; set; } = string.Empty;
    public virtual void MakeSound() => Console.WriteLine($"{Name}: Generic animal sound");
}

public class Dog : Animal
{
    public override void MakeSound() => Console.WriteLine($"{Name}: Woof! 🐕");
}

public class Cat : Animal
{
    public override void MakeSound() => Console.WriteLine($"{Name}: Meow! 🐱");
}

// Covariance (out) - Producer
public interface IProducer<out T>
{
    T Produce();  // T sadece return type'da (output)
    // void Consume(T item);  // ❌ Error! T input position'da olamaz
}

// Contravariance (in) - Consumer
public interface IConsumer<in T>
{
    void Consume(T item);  // T sadece parameter'da (input)
    // T Produce();  // ❌ Error! T return position'da olamaz
}

// Invariance - Neither in nor out
public interface IProcessor<T>
{
    T Process(T item);  // T hem input hem output - invariant
}

public class DogProducer : IProducer<Dog>
{
    public Dog Produce()
    {
        Console.WriteLine("🏭 Producing a Dog");
        return new Dog { Name = "Buddy" };
    }
}

public class AnimalConsumer : IConsumer<Animal>
{
    public void Consume(Animal animal)
    {
        Console.WriteLine($"🍽️  Consuming animal: {animal.Name}");
        animal.MakeSound();
    }
}

public class AnimalProcessor : IProcessor<Animal>
{
    public Animal Process(Animal animal)
    {
        Console.WriteLine($"⚙️  Processing: {animal.Name}");
        return animal;
    }
}
