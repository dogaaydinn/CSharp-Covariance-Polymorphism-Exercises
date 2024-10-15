namespace ConsoleApp2.Beginner;

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

    public AssignmentCompatibility()
    {
        str = (string)obj;
    }

    // 2. yol -Type Checking with is Keyword:
    // Bu yöntem, obj nesnesinin türünü kontrol etmek ve aynı anda dönüştürmek için is anahtar kelimesini kullanır. Bu yöntem daha güvenli ve temiz bir yaklaşımdır.

    public void TypeChecking()
    {
        if (obj is string)
            str = (string)obj;
        else
            str = null;
    }
}