namespace AdvancedCsharpConcepts.Intermediate.CovarianceContravariance;

/// <summary>
/// Demonstrates covariance and contravariance concepts in C#.
/// </summary>
/// <remarks>
/// Covariance: Allows using a more derived type (IEnumerable&lt;Derived&gt; → IEnumerable&lt;Base&gt;)
/// Contravariance: Allows using a less derived type (Action&lt;Base&gt; → Action&lt;Derived&gt;)
/// </remarks>
public class CovarianceContravariance
{
    //IEnumerable<string> türünde bir liste oluştur
    //bunu IEnumerable<object> türüne atamaya çalışın. Bu işlemin nasıl başarılı olduğunu açıklayın.

    /// <summary>
    /// Demonstrates covariance by converting IEnumerable&lt;string&gt; to IEnumerable&lt;object&gt;.
    /// </summary>
    /// <remarks>
    /// This works because IEnumerable&lt;T&gt; is covariant (has 'out' keyword on T).
    /// Covariance is safe for read-only operations since you're only getting items out.
    /// </remarks>
    public void CovarianceExample()
    {
        // IEnumerable<string> türünde bir liste oluşturuluyor
        IEnumerable<string> stringList = new List<string>
        {
            "Alice",
            "Bob",
            "Charlie"
        };
        Console.WriteLine("Original string list:");
        foreach (var item in stringList) Console.WriteLine(item);

        // IEnumerable<object> türünde bir liste oluşturuluyor
        IEnumerable<object> mixedList = new List<object>
        {
            "Alice", // string
            42, // int
            3.14, // double
            new DateTime(2023, 1, 1) // DateTime
        };
        Console.WriteLine("\nMixed type list:");
        foreach (var item in mixedList) Console.WriteLine(item);

        // IEnumerable<object> türüne cast ediliyor (upcasting)
        IEnumerable<object> objectList = stringList;
        Console.WriteLine("\nCasted string list to IEnumerable<object>:");
        foreach (var item in objectList) Console.WriteLine(item);

        // stringList içeriği döngü ile yazdırılıyor
        Console.WriteLine("\nOriginal string list again:");
        foreach (var item in stringList) Console.WriteLine(item);
    }
    //soru 2: Action<object> türünde bir delege oluşturun ve bunu Action<string> ile atamayı deneyin. Neden derleme hatası alındığını açıklayın.

    /// <summary>
    /// Demonstrates contravariance with Action delegates and explains why certain conversions fail.
    /// </summary>
    /// <remarks>
    /// Action&lt;T&gt; is contravariant (has 'in' keyword on T), but in the opposite direction.
    /// You can assign Action&lt;object&gt; to Action&lt;string&gt; in contravariant scenarios,
    /// but not Action&lt;string&gt; to Action&lt;object&gt;.
    /// This is safe because a method accepting object can safely accept any string.
    /// </remarks>
    public void ContravarianceExample()
    {
        // Action<object> türünde bir delege oluşturuluyor
        Action<object> objectAction = obj => Console.WriteLine($"Object action: {obj}");

        // Action<string> türüne cast ediliyor (downcasting)
        // Action<string> stringAction = objectAction; // Hata verir

        // Action<string> türüne cast edilirken, contravariance kuralı ihlal edildiği için hata alınır.
        // Contravariance, parametre türlerinin tersine çevrilemeyeceğini belirtir.
        // Yani, Action<object> türündeki bir delege, Action<string> türüne cast edilemez.

        // Bu kodu çalıştırdığınızda, aşağıdaki satır derleme hatası verecektir:
        // Action<string> stringAction = objectAction;

        // Bunun yerine, Action<string> türünde bir delege oluşturup kullanabiliriz:
        Action<string> stringAction = str => Console.WriteLine($"String action: {str}");

        // Delege çağrıları
        objectAction("Hello from objectAction");
        stringAction("Hello from stringAction");
    }
}