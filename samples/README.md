# Sample Projects - Örnek Projeler

Modern C# 12 ve .NET 8 kullanarak hazırlanmış, beginner'dan intermediate seviyeye kadar 18 eğitim projesi.

## 📚 Proje Yapısı

### 01-Beginner (10 Proje)
Temel C# kavramlarını öğreten başlangıç seviyesi projeler.

| # | Proje Adı | Konu | Durum |
|---|-----------|------|-------|
| 1 | **PolymorphismBasics** | Virtual/override, base class | ✅ Tamamlandı |
| 2 | **CastingExamples** | as, is, pattern matching | ✅ Tamamlandı |
| 3 | OverrideVirtual | Method hiding vs override | 📋 Planlı |
| 4 | InterfaceBasics | Interface implementation | 📋 Planlı |
| 5 | AbstractClassExample | Abstract class vs interface | 📋 Planlı |
| 6 | TypeChecking | GetType(), typeof, is | 📋 Planlı |
| 7 | MethodOverloading | Parametre overloading | 📋 Planlı |
| 8 | ConstructorChaining | this(), base() kullanımı | 📋 Planlı |
| 9 | PropertyExamples | Auto-property, validation | 📋 Planlı |
| 10 | IndexerExample | Class indexer | 📋 Planlı |

### 02-Intermediate (8 Proje)
Orta seviye C# konularını kapsayan projeler.

| # | Proje Adı | Konu | Durum |
|---|-----------|------|-------|
| 11 | GenericConstraints | where T: constraints | 📋 Planlı |
| 12 | CovarianceContravariance | out/in modifiers | 📋 Planlı |
| 13 | BoxingPerformance | Value vs reference types | 📋 Planlı |
| 14 | NullableReferenceTypes | Nullable context | 📋 Planlı |
| 15 | PatternMatching | Switch expressions | 📋 Planlı |
| 16 | ExtensionMethods | Static class extensions | 📋 Planlı |
| 17 | DelegateExample | Func, Action, Predicate | 📋 Planlı |
| 18 | EventHandlerPattern | Event handling | 📋 Planlı |

**İlerleme**: 2/18 (11% tamamlandı)

---

## 🚀 Tamamlanan Projeler

### 1. PolymorphismBasics ✅
Hayvanat bahçesi yönetim sistemi ile polimorfizm temellerini öğrenin.

**Özellikler:**
- Virtual/override metodlar
- Polymorphic collections
- Base class kullanımı
- 5 dosya, ~250 satır kod
- Çalışan örnek çıktı

**Çalıştırma:**
```bash
cd 01-Beginner/PolymorphismBasics
dotnet run
```

**Öğrenilen Kavramlar:**
- ✅ Virtual methods
- ✅ Override keyword
- ✅ Polymorphic behavior
- ✅ Liskov Substitution Principle

---

### 2. CastingExamples ✅
Çalışan yönetim sistemi ile güvenli tip dönüşümlerini öğrenin.

**Özellikler:**
- `as` operatörü (güvenli)
- `is` operatörü (type checking)
- Pattern matching (modern)
- Switch expressions
- 5 dosya, ~240 satır kod

**Çalıştırma:**
```bash
cd 01-Beginner/CastingExamples
dotnet run
```

**Öğrenilen Kavramlar:**
- ✅ Safe downcasting
- ✅ Pattern matching
- ✅ Type checking
- ✅ Explicit vs implicit casting

---

## 📁 Proje Dosya Yapısı

Her proje şu dosyaları içerir:

```
ProjectName/
├── ProjectName.csproj         # .NET 8, C# 12 konfigürasyonu
├── README.md                  # Kullanım, örnekler, öğrenilen kavramlar
├── WHY_THIS_PATTERN.md        # Neden bu pattern, avantajlar, best practices
├── Program.cs                 # Ana çalıştırılabilir kod
├── MainClass.cs               # Ana domain sınıfı
└── SupportClass.cs            # Yardımcı sınıflar
```

**Standartlar:**
- ✅ .NET 8 SDK
- ✅ C# 12 language features
- ✅ Nullable reference types enabled
- ✅ Modern syntax (primary constructors, collection expressions)
- ✅ Türkçe açıklama yorumları
- ✅ Her dosya max 250 satır

---

## 🎯 Proje Özellikleri

### Kod Kalitesi
- **Modern C# 12**: Primary constructors, pattern matching, switch expressions
- **Best Practices**: SOLID principles, clean code, separation of concerns
- **Performance**: Performans notları ve optimizasyon önerileri
- **Documentation**: Her satır açıklanmış, öğrenme odaklı

### Öğrenme Yaklaşımı
Her `Program.cs` şu formatı takip eder:

```csharp
// SCENARIO: [Ne öğretiliyor]
// BAD PRACTICE: [Kötü yaklaşım örneği]
// GOOD PRACTICE: [İyi yaklaşım örneği]

// Kod örnekleri...

// === Output Analysis ===
// 1. [Analiz noktası 1]
// 2. [Analiz noktası 2]
// 3. [Analiz noktası 3]
```

### WHY_THIS_PATTERN.md İçeriği
- 🤔 Problem: Çözülmeye çalışılan sorun
- ❌ Kötü Yaklaşım: Anti-pattern örnekleri
- ✅ İyi Yaklaşım: Best practice örnekleri
- ✨ Faydalar: Pattern'in avantajları
- 🏗️ Gerçek Dünya: Production kullanımları
- 📊 Ne Zaman: Kullanım senaryoları

---

## 💻 Nasıl Kullanılır?

### Tüm Projeleri Klonla
```bash
git clone https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises.git
cd CSharp-Covariance-Polymorphism-Exercises/samples
```

### Bir Projeyi Çalıştır
```bash
cd 01-Beginner/PolymorphismBasics
dotnet restore
dotnet build
dotnet run
```

### Tüm Projeleri Test Et
```bash
# Her proje için build yap
for dir in 01-Beginner/*/; do
    echo "Building $dir"
    cd "$dir" && dotnet build && cd -
done
```

---

## 📚 Öğrenme Yolu

### Başlangıç Seviyesi (1-2 Ay)
1. **PolymorphismBasics**: Polimorfizm temellerini öğren
2. **CastingExamples**: Güvenli tip dönüşümlerini öğren
3. **OverrideVirtual**: Override vs hiding farkını anla
4. **InterfaceBasics**: Interface kullanımını öğren
5. **AbstractClassExample**: Abstract class vs interface

**Sonra**: Temel OOP kavramlarını pekiştir

### Orta Seviye (2-3 Ay)
6. **TypeChecking**: Runtime type checking
7. **MethodOverloading**: Overloading patterns
8. **ConstructorChaining**: Constructor best practices
9. **PropertyExamples**: Modern property patterns
10. **IndexerExample**: Custom indexers

**Sonra**: Generic ve advanced konulara geç

### İleri Seviye (1-2 Ay)
11-18. Intermediate projeleri tamamla

**Sonra**: Production-ready kod yazmaya hazırsın!

---

## 🎓 Önkoşullar

- **.NET 8 SDK** yüklü olmalı
- **C# temel bilgisi** (değişkenler, loops, methods)
- **OOP kavramları** (class, inheritance) hakkında fikir
- **IDE**: Visual Studio 2022 / Rider / VS Code

### Kurulum Kontrolü
```bash
dotnet --version  # 8.0.x görmeli
```

---

## 📖 İlgili Kaynaklar

- [C# Documentation](https://docs.microsoft.com/dotnet/csharp/)
- [.NET 8 Release Notes](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-8)
- [C# Language Features](https://docs.microsoft.com/dotnet/csharp/whats-new/csharp-12)

### Proje İçi Bağlantılar
- [Project Specifications](./PROJECT_SPECIFICATIONS.md) - Tüm projelerin detaylı spesifikasyonları
- [Main README](../README.md) - Ana proje dokümantasyonu
- [GETTING_STARTED](../GETTING_STARTED.md) - Başlangıç rehberi

---

## 🤝 Katkıda Bulunma

Yeni proje eklemek veya mevcut projeleri geliştirmek için:

1. Bu repository'yi fork edin
2. Yeni branch oluşturun (`git checkout -b feature/YeniProje`)
3. Değişiklikleri commit edin (`git commit -m 'feat: yeni proje eklendi'`)
4. Branch'i push edin (`git push origin feature/YeniProje`)
5. Pull Request açın

**Proje Standartları:**
- .NET 8 ve C# 12 kullanın
- Her dosya max 250 satır
- Türkçe yorumlar ekleyin
- README.md ve WHY_THIS_PATTERN.md dahil edin
- Çalışan örnek kod sağlayın

---

## 📝 Lisans

MIT License - Detaylar için [LICENSE](../LICENSE) dosyasına bakın.

---

## 📞 İletişim

- **GitHub Issues**: [Sorun bildirin](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/issues)
- **Discussions**: [Tartışmaya katılın](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/discussions)

---

**Son Güncelleme**: Aralık 2024
**Durum**: 🚧 Aktif Geliştirme (2/18 proje tamamlandı)
**Hedef**: 18/18 proje tamamlanması
