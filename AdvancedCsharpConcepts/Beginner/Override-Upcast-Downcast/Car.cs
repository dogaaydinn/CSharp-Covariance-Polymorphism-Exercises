namespace AdvancedCsharpConcepts.Beginner.Override_Upcast_Downcast;

public class Car : Vehicle
{
    public override void Drive()
    {
        Console.WriteLine("Car is driving");
    }

    public override void DisplayInfo()
    {
        Console.WriteLine("Car info");
    }

    //Soru 2: Car sınıfındaki bir nesneyi Vehicle türüne cast et ve ardından tekrar Car türüne geri cast et. 
    public static void CastExample()
    {
        // Car nesnesi oluşturuluyor
        var myCar = new Car();
        Console.WriteLine("Car object created.");

        // Car nesnesi Vehicle türüne cast ediliyor (upcasting)
        Vehicle myVehicle = myCar;
        Console.WriteLine("Car object cast to Vehicle (upcasting).");

        // Vehicle türündeki nesne tekrar Car türüne cast ediliyor (downcasting)
        if (myVehicle is Car myNewCar)
        {
            Console.WriteLine("Vehicle object cast back to Car (downcasting).");
            myNewCar.DisplayInfo(); // Güvenli bir şekilde Car nesnesine ait metod çağrılır
        }
        else
        {
            Console.WriteLine("myVehicle is not a Car.");
        }

        // Alternatif yöntem: explicit casting ile downcasting
        try
        {
            var myExplicitCar = (Car)myVehicle; // Cast işlemine güveniyoruz
            Console.WriteLine("Explicit downcast from Vehicle to Car successful.");
            myExplicitCar.DisplayInfo(); // Güvenli metod çağrısı
        }
        catch (InvalidCastException)
        {
            Console.WriteLine("Explicit downcast failed.");
        }
    }

/*
 Upcasting (Yukarı Doğru Tür Değiştirme):

   Car nesnesi Vehicle türüne cast edildiğinde, myVehicle referansı artık Vehicle türünde bir nesne olarak görülür.
    Bu işlem başarılıdır çünkü Car sınıfı Vehicle sınıfından türemiştir.
     Yani her Car, bir Vehicle olarak kabul edilebilir.
      Upcasting otomatik olarak yapılabilir ve genellikle güvenlidir, çünkü Car nesnesi hala Vehicle türüne aittir.
 */

/*
 Downcasting (Aşağı Doğru Tür Değiştirme):

   myVehicle referansını Car türüne geri cast ettiğinizde, bu işlem (Car)myVehicle şeklinde yapılır.
    Bu işlem başarılıdır çünkü myVehicle aslında bir Car nesnesini referans alıyor.
    Ancak, bu işlem InvalidCastException hatasına yol açabilir
    eğer myVehicle referansı Car türünde bir nesneye işaret etmiyorsa.
    Bu nedenle, downcasting yapmadan önce, is veya as anahtar kelimeleri ile tür kontrolü yapılması önerilir.
 */
}