namespace Contravariance;

// Base class hierarchy for demonstrating contravariance
public class Animal
{
    public string Name { get; set; }
    public int Age { get; set; }
    public double Weight { get; set; }

    public Animal(string name, int age, double weight)
    {
        Name = name;
        Age = age;
        Weight = weight;
    }

    public virtual string MakeSound() => "Some generic animal sound";
}

public class Mammal : Animal
{
    public bool HasFur { get; set; }

    public Mammal(string name, int age, double weight, bool hasFur)
        : base(name, age, weight)
    {
        HasFur = hasFur;
    }
}

public class Dog : Mammal
{
    public string Breed { get; set; }

    public Dog(string name, int age, double weight, string breed)
        : base(name, age, weight, true)
    {
        Breed = breed;
    }

    public override string MakeSound() => "Woof!";
}

public class Cat : Mammal
{
    public bool IsIndoor { get; set; }

    public Cat(string name, int age, double weight, bool isIndoor)
        : base(name, age, weight, true)
    {
        IsIndoor = isIndoor;
    }

    public override string MakeSound() => "Meow!";
}

// For event handler examples
public class AnimalEventArgs : EventArgs
{
    public Animal Animal { get; set; }
    public string EventType { get; set; }

    public AnimalEventArgs(Animal animal, string eventType)
    {
        Animal = animal;
        EventType = eventType;
    }
}
