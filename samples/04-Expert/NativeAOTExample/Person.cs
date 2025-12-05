namespace NativeAOTExample;

/// <summary>
/// Person model - simple, AOT-compatible.
/// Native AOT requires simple types without complex reflection dependencies.
/// </summary>
public class Person
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }

    public override string ToString()
    {
        return $"{Name} (Age: {Age}, Email: {Email})";
    }
}
