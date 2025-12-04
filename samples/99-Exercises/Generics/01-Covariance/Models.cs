namespace Covariance;

// Base class hierarchy for demonstrating covariance
public class Animal
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Animal(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public virtual string MakeSound() => "Some generic animal sound";
}

public class Mammal : Animal
{
    public bool HasFur { get; set; }

    public Mammal(string name, int age, bool hasFur) : base(name, age)
    {
        HasFur = hasFur;
    }
}

public class Dog : Mammal
{
    public string Breed { get; set; }

    public Dog(string name, int age, string breed) : base(name, age, true)
    {
        Breed = breed;
    }

    public override string MakeSound() => "Woof!";
}

public class Cat : Mammal
{
    public bool IsIndoor { get; set; }

    public Cat(string name, int age, bool isIndoor) : base(name, age, true)
    {
        IsIndoor = isIndoor;
    }

    public override string MakeSound() => "Meow!";
}

public class Bird : Animal
{
    public bool CanFly { get; set; }

    public Bird(string name, int age, bool canFly) : base(name, age)
    {
        CanFly = canFly;
    }

    public override string MakeSound() => "Tweet!";
}
