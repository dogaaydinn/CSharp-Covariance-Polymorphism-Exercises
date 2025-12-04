namespace ConstructorChaining;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }

    public Person()
    {
        Console.WriteLine("→ Person() parametre constructor");
        Name = "Unknown";
        Age = 0;
        Address = "Unknown";
    }

    public Person(string name) : this()
    {
        Console.WriteLine($"→ Person(name) constructor: {name}");
        Name = name;
    }

    public Person(string name, int age) : this(name)
    {
        Console.WriteLine($"→ Person(name, age) constructor: age={age}");
        Age = age;
    }

    public Person(string name, int age, string address) : this(name, age)
    {
        Console.WriteLine($"→ Person(name, age, address) constructor: address={address}");
        Address = address;
    }
}

public class Employee : Person
{
    public string Department { get; set; }
    public decimal Salary { get; set; }

    public Employee() : base()
    {
        Console.WriteLine("→ Employee() constructor");
        Department = "Unknown";
    }

    public Employee(string name, int age) : base(name, age)
    {
        Console.WriteLine("→ Employee(name, age) constructor");
        Department = "General";
    }

    public Employee(string name, int age, string department, decimal salary)
        : base(name, age)
    {
        Console.WriteLine($"→ Employee(full) constructor: dept={department}, salary={salary}");
        Department = department;
        Salary = salary;
    }
}

public class Manager : Employee
{
    public List<Employee> Team { get; set; } = new();
    public decimal Bonus { get; set; }

    public Manager() : base()
    {
        Console.WriteLine("→ Manager() constructor");
    }

    public Manager(string name, int age, string department, decimal salary, decimal bonus)
        : base(name, age, department, salary)
    {
        Console.WriteLine($"→ Manager(full) constructor: bonus={bonus}");
        Bonus = bonus;
    }
}
