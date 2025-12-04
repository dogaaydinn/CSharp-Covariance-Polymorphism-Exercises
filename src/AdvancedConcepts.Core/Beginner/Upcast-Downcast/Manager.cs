namespace AdvancedCsharpConcepts.Beginner.Upcast_Downcast;

public class Manager : Employee
{
    //Manager nesnesini Employee türüne atayıp, Manager’a geri çevirin (downcast). Bu işlemin başarı durumunu kontrol edin.

    private int _bonus;


    public static void DownCast()
    {
        // Manager nesnesi oluşturuluyor
        var manager = new Manager
        {
            Name = "Alice",
            Age = 35,
            _bonus = 5000
        };

        // Manager nesnesi Employee türüne cast ediliyor (upcasting)
        Employee myEmployee = manager;

        // Upcasting sonrası erişilebilecek özellikler
        Console.WriteLine("Accessing properties after upcasting:");
        myEmployee.DisplayInfo(); // Erişim: Name ve Age

        // myEmployee.Bonus; // Bu erişim hatalı olur; çünkü Bonus, sadece Manager için geçerli
        // Downcasting
        if (myEmployee is Manager myCheckedManager)
        {
            Console.WriteLine("Downcasting successful:");
            Console.WriteLine($"Bonus: {myCheckedManager._bonus}");
        }
        else
        {
            Console.WriteLine("Downcasting failed.");
        }
    }
}