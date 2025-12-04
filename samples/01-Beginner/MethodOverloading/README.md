# Method Overloading - Parametre Overloading

## 📚 Konu
Aynı isimli metodun farklı parametre imzalarıyla tanımlanması.

## 🔑 Kavramlar
- **Overload**: Aynı isim, farklı parametreler
- **Overload Resolution**: Compiler'ın doğru metodu seçmesi
- **Optional Parameters**: Varsayılan değerler
- **Params Keyword**: Variable arguments
- **Named Arguments**: Parametre ismi ile çağrı

## 💻 Kullanım
```bash
cd samples/01-Beginner/MethodOverloading
dotnet run
```

## 🎓 Örnekler
```csharp
// Parametre sayısı
int Add(int a, int b) { }
int Add(int a, int b, int c) { }

// Parametre türü
double Add(double a, double b) { }

// Optional
int Multiply(int a, int b = 1) { }

// Params
int Add(params int[] numbers) { }

// Named arguments
Calculate(value: 100, rate: 0.05, years: 10);
```

## 💡 Best Practices
- Tutarlı isimlendirme
- En spesifik overload önce
- Optional params dikkatli kullan
- Return type overload için yeterli DEĞİL
