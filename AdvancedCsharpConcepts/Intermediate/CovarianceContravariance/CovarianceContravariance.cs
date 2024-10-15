namespace AdvancedCsharpConcepts.Intermediate.CovarianceContravariance;

public class CovarianceContravariance
{
    //IEnumerable<string> türünde bir liste oluştur
    //bunu IEnumerable<object> türüne atamaya çalışın. Bu işlemin nasıl başarılı olduğunu açıklayın.

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