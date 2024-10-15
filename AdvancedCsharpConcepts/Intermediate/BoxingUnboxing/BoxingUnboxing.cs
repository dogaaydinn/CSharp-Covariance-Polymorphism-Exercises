using System.Collections;

namespace AdvancedCsharpConcepts.Intermediate.BoxingUnboxing;

public class BoxingUnboxing
{
    //Soru 1: Bir int değişkeni boxing ile object türüne dönüştürün. Daha sonra bunu tekrar int türüne unboxing yapın. Bu işlemler sırasında hafıza yönetimi açısından neler olduğunu açıklayın.

    public static void BoxingUnboxingExample()
    {
        // int değişkeni oluşturuluyor
        var myInt = 123;
        double myDouble = 123.456;
        // Boxing: int türündeki değişken object türüne cast ediliyor
        object myObject = myInt;
        object myObject2 = myDouble;
        // Unboxing: object türündeki değişken int türüne cast ediliyor
        var myNewInt = (int)myObject;
        int myNewInt2 = (int)(double)myObject2;
        // Boxing işlemi, değeri heap belleğinde bir nesne olarak saklar.
        // Unboxing işlemi, heap belleğindeki nesneyi alır ve değeri geri alır.
        // Boxing ve unboxing işlemleri, performans açısından maliyetli olabilir ve gereksiz yere bellek kullanımına yol açabilir.
        Console.WriteLine($"Original double: {myDouble}");
        Console.WriteLine($"Boxed object: {myObject}");
        Console.WriteLine($"Unboxed int (with data loss): {myNewInt}");
    }

    //Soru 2: ArrayList kullanarak farklı türlerden veriler ekleyin (örneğin int ve string). Ardından bu verileri unboxing ile geri alın ve performans farklarını analiz edin.

    public static void ArrayListExample()
    {
        // ArrayList oluşturuluyor
        var myList = new ArrayList();

        // ArrayList'e farklı türlerde veriler ekleniyor
        myList.Add(123); // int
        myList.Add("Hello, World!"); // string
        myList.Add(123.456); // double

        // ArrayList'ten veriler unboxing ile geri alınıyor
        foreach (var item in myList)
        {
            switch (item)
            {
                case int myInt:
                    Console.WriteLine($"Unboxed int: {myInt}");
                    break;
                case string myString:
                    Console.WriteLine($"Unboxed string: {myString}");
                    break;
                case double myDouble:
                {
                    // Unboxing double to int (veri kaybı)
                    int myNewInt = (int)myDouble;
                    Console.WriteLine($"Unboxed double: {myDouble}");
                    Console.WriteLine($"Unboxed int (with data loss): {myNewInt}");
                    break;
                }
            }
            // ArrayList, farklı türlerdeki verileri aynı listede tutabilir.
            // Ancak, unboxing işlemi her seferinde tür kontrolü yaparak gerçekleştirildiğinden performans açısından maliyetli olabilir.
        }
    }
}
