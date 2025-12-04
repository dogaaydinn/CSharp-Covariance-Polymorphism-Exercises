# Polymorphism Basics - Polimorfizm Temelleri

## 📚 Konu
Virtual metodlar, override anahtar kelimesi ve polimorfik davranış.

## 🎯 Amaç
Hayvanat bahçesi yönetim sistemi örneği ile polimorfizmin temel prensiplerint öğrenmek.

## 🔑 Anahtar Kavramlar
- **Virtual Methods**: Base class'ta tanımlanan, türetilmiş sınıflarca özelleştirilebilir metodlar
- **Override**: Türetilmiş sınıfta virtual metodun yeniden yazılması
- **Polymorphic Behavior**: Aynı arayüzle farklı davranışlar sergileme
- **Base Class**: Ortak özellikleri tanımlayan üst sınıf

## 💻 Kullanım

```bash
# Projeyi çalıştır
cd samples/01-Beginner/PolymorphismBasics
dotnet run
```

## 📊 Örnek Çıktı

```
=== Polimorfizm Temel Örneği: Hayvanat Bahçesi ===

✅ Simba hayvanat bahçesine eklendi.
✅ Dumbo hayvanat bahçesine eklendi.
✅ Abu hayvanat bahçesine eklendi.

🍽️  === Doğa Vadisi Hayvanat Bahçesi - Beslenme Zamanı ===
Simba: 🦁 ROAAAAR! (Kükreme sesi - Savana Bölgesinden)
   Simba besleniyor...

Dumbo: 🐘 PAAAOOO! (Boru sesi - 1.8m fildişi)
   Dumbo besleniyor...

Abu: 🐵 OOH OOH AAH AAH! (En sevdiği yemek: Muz)
   Abu besleniyor...

🏃 === Doğa Vadisi Hayvanat Bahçesi - Egzersiz Zamanı ===
Simba güçlü adımlarla yürüyor ve bölgesini kontrol ediyor.

Dumbo ağır ama görkemli adımlarla yürüyor.

Abu ağaçtan ağaca atlayarak hızla hareket ediyor.

🎪 === Doğa Vadisi Hayvanat Bahçesi - Özel Aktiviteler ===
Ad: Simba, Yaş: 5
Simba avını takip ediyor... 🎯

Ad: Dumbo, Yaş: 12
Dumbo hortumu ile su püskürtüyor! 💦

Ad: Abu, Yaş: 3
Abu liandan sarılarak sallanıyor! 🌿

📊 === Doğa Vadisi Hayvanat Bahçesi - İstatistikler ===
Toplam Hayvan Sayısı: 3
Aslan: 1
Fil: 1
Maymun: 1
```

## 🎓 Öğrenilen Kavramlar

### 1. Virtual Methods
```csharp
public abstract class Animal
{
    public virtual void MakeSound() { }  // Override edilebilir
}
```

### 2. Override Keyword
```csharp
public class Lion : Animal
{
    public override void MakeSound()     // Base metodun yeniden tanımı
    {
        Console.WriteLine("ROAAR!");
    }
}
```

### 3. Polymorphic Collections
```csharp
List<Animal> animals = new()
{
    new Lion("Simba", 5),
    new Elephant("Dumbo", 12)
};

foreach (var animal in animals)
{
    animal.MakeSound();  // Her hayvan kendi sesini çıkarır
}
```

## ⚡ Performans Notları

1. **Virtual Method Overhead**: ~5% performans maliyeti (vtable lookup)
2. **Memory**: Virtual metod pointer'ları için minimal ekstra bellek
3. **Best Practice**: Performans kritik olmayan durumlarda esneklik için virtual kullan

## 🔄 İlişkili Konular
- [CastingExamples](../CastingExamples/) - Tip dönüşümleri
- [OverrideVirtual](../OverrideVirtual/) - Override vs method hiding
- [AbstractClassExample](../AbstractClassExample/) - Abstract class kullanımı
