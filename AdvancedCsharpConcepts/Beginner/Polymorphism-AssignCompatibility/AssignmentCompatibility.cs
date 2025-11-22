namespace AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

/// <summary>
/// Demonstrates assignment compatibility and type casting in C#.
/// Shows how to safely downcast from object to string using different approaches.
/// </summary>
public class AssignmentCompatibility
{
    // string object classından türediği için implicit olarak cast edilebilir. ve bir hata alınmaz
    private readonly object obj = "Hello, World!";
    // string str = obj;  hata verdi

    //  string str = (string) obj; obj static olmasını istediğinden hata verdi
    /*
     string str = obj; satırı, burada bir object türündeki obj değişkeninin string türüne atanmasıdır.
      Ancak, bu işlem çalışmaz ve bir derleme hatası verir.
       C# dilinde bu tür bir atama, explicit downcasting gerektirir.
       Yani, obj değişkeninin gerçekten bir string türünde olup olmadığını kontrol etmek ve ona göre casting (dönüştürme) yapmak gerekir.
     */

    // 1.yol - Explicit Cast in Constructor:
    // Bu yöntem, obj nesnesini string türüne açıkça dönüştürmek için kullanılır. Bu dönüşüm, sınıfın yapıcısında (constructor) yapılır.

    private string str;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignmentCompatibility"/> class.
    /// Performs explicit cast from object to string.
    /// </summary>
    public AssignmentCompatibility()
    {
        str = (string)obj;
    }

    // 2. yol -Type Checking with is Keyword:
    // Bu yöntem, obj nesnesinin türünü kontrol etmek ve aynı anda dönüştürmek için is anahtar kelimesini kullanır. Bu yöntem daha güvenli ve temiz bir yaklaşımdır.

    /// <summary>
    /// Demonstrates safe type checking and casting using modern pattern matching.
    /// Uses 'is' keyword with pattern matching (C# 7.0+) for type-safe conversion.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when obj is not a string.</exception>
    public void TypeChecking()
    {
        if (obj is string stringValue)
        {
            str = stringValue;
        }
        else
        {
            throw new InvalidOperationException("Object is not a string");
        }
    }

    /// <summary>
    /// Alternative approach using as operator with null-conditional handling.
    /// </summary>
    /// <returns>The string value if conversion succeeds, or throws exception.</returns>
    /// <exception cref="InvalidOperationException">Thrown when obj is not a string.</exception>
    public string GetStringValue()
    {
        return obj as string ?? throw new InvalidOperationException("Object is not a string");
    }
}
