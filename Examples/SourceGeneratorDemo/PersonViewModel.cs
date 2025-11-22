using SourceGenerators.Attributes;

namespace SourceGeneratorDemo;

/// <summary>
/// Example ViewModel demonstrating AutoNotifyGenerator.
/// The [AutoNotify] attribute automatically generates properties with INotifyPropertyChanged implementation.
/// </summary>
public partial class PersonViewModel
{
    [AutoNotify]
    private string _firstName = string.Empty;

    [AutoNotify]
    private string _lastName = string.Empty;

    [AutoNotify]
    private int _age;

    [AutoNotify]
    private string _email = string.Empty;

    [AutoNotify]
    private bool _isActive;

    // Regular property without AutoNotify
    public string FullName => $"{FirstName} {LastName}";

    public PersonViewModel()
    {
        // Subscribe to property changes
        PropertyChanged += (sender, e) =>
        {
            Console.WriteLine($"Property changed: {e.PropertyName}");
        };
    }
}
