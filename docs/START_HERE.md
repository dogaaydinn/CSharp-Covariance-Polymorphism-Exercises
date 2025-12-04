# 🎯 NEREDEN BAŞLAMALI? - MERKEZI ÖĞRENME HARITASI

**"Bu repo'da kayboldum, nereden başlamalıyım?"** - İşte net cevabı.

---

## 📍 Hızlı Başlangıç (5 dakika)

### Adım 1: Seviyeni Belirle

**Aşağıdaki sorulara cevap ver:**

- [ ] C# syntax'ını biliyor musun? (class, method, if/else)
- [ ] OOP'yi anlıyor musun? (inheritance, polymorphism)
- [ ] Design patterns'lerle çalıştın mı?
- [ ] Production'da mikroservis geliştirdin mi?

**Sonuç:**
- **0 evet** → YOL 1: Sıfırdan Junior (sayfa 10'a git)
- **1-2 evet** → YOL 2: Junior'dan Mid-Level (sayfa 45'e git)
- **3 evet** → YOL 3: Mid-Level'dan Senior (sayfa 95'e git)
- **Interview hazırlığı?** → YOL 4: Mülakat Prep (sayfa 150'ye git)

---

### Adım 2: İlk 3 Dosyayı Aç

Hangi yolu seçersen seç, ilk 3 dosya HER ZAMAN aynı:

1. **Bu dosya** - `docs/START_HERE.md` (şu an okuyorsun)
2. **Senin yolun** - `docs/LEARNING_PATHS.md` içinde ilgili bölüm
3. **İlk sample** - Yoluna göre değişir

---

### Adım 3: İlk Saatte Yapılacaklar

**Yol 1 (Başlangıç):**
```bash
1. Aç: samples/01-Beginner/PolymorphismBasics/
2. Oku: WHY_THIS_PATTERN.md
3. Çalıştır: 01_SimplePolymorphism.cs
4. Anla: Polymorphism nedir, neden önemli?
Süre: ~60 dakika
```

**Yol 2 (Junior → Mid):**
```bash
1. Aç: samples/03-Advanced/DesignPatterns/
2. Oku: WHY_THIS_PATTERN.md
3. Çalıştır: FactoryPattern.cs
4. Uygula: Kendi projenizde Factory örneği bulun
Süre: ~60 dakika
```

**Yol 3 (Mid → Senior):**
```bash
1. Aç: samples/98-RealWorld-Problems/01-API-Rate-Limiting/
2. Oku: PROBLEM.md
3. Çöz: Kendi çözümünü yaz (bakmadan)
4. Karşılaştır: SOLUTION-A.md ile karşılaştır
Süre: ~90 dakika
```

**Yol 4 (Mülakat):**
```bash
1. Aç: docs/code-reviews/01-Polymorphism-Review/
2. Oku: bad-code.cs
3. Düşün: Sen olsan nasıl review yapardın?
4. Karşılaştır: review-comments.md ile karşılaştır
Süre: ~45 dakika
```

---

## 📚 4 ANA ÖĞRENME YOLU

| Yol | Başlangıç | Hedef | Süre | Hafta/Saat | Ön Koşul |
|-----|-----------|-------|------|------------|----------|
| **[YOL 1](#-yol-1-sifirdan-junior-developera)** | Hiç C# bilmiyorum | Junior Developer | 3-6 ay | 10-15h | Programlama temelleri |
| **[YOL 2](#-yol-2-juniordan-mid-levela)** | Temel C# biliyorum | Mid-Level | 6-9 ay | 8-12h | Yol 1 VEYA eşdeğer |
| **[YOL 3](#-yol-3-mid-leveldan-seniora)** | 2-3 yıl deneyim | Senior Developer | 9-12 ay | 6-10h | Yol 2 VEYA eşdeğer |
| **[YOL 4](#-yol-4-mulakata-hazirlik)** | Herhangi bir seviye | Interview Ready | 1-2 ay | 15-20h (yoğun) | İlgili yol tamamlanmış |

---

## 🚀 YOL 1: SIFIRDAN JUNIOR DEVELOPER'A

**👤 Hedef Kitle:** 
- Hiç C# bilmeyenler
- Başka dilden geçiş yapanlar (Python, Java, etc.)
- CS öğrencileri

**📋 Ön Koşullar:**
- Programlama mantığını biliyorsun (değişken, döngü, fonksiyon)
- VS Code veya Visual Studio yüklü
- Git temelleri (commit, push)

**⏱️ Süre:** 3-6 ay (haftada 10-15 saat çalışma ile)

**🎯 Çıktı:**
- Junior .NET Developer pozisyonuna başvurabilme
- Basit REST API'lar yazabilme
- CRUD uygulamaları geliştirebilme
- Junior-level teknik mülakata girebilme

**💰 Hedef Maaş:** $60,000-$80,000/yıl (ABD) veya ₺40,000-₺60,000/ay (Türkiye)

---

### 📅 HAFTA HAFTA DETAYLI PLAN (Yol 1)

#### HAFTA 1: C# Temelleri ve İlk Polymorphism

**🎯 Bu Hafta Ne Öğreneceksin:**
- C# syntax temellerini
- Virtual ve override kavramlarını
- İlk polymorphic kodunu yazacaksın

**📖 Okuma Materyalleri (3 saat):**
1. `samples/01-Beginner/PolymorphismBasics/WHY_THIS_PATTERN.md`
   - Polymorphism nedir?
   - Neden if/else zincirleri kötü?
   - Gerçek dünya örnekleri
   
2. `docs/code-reviews/01-Polymorphism-Review/bad-code.cs`
   - Kötü kodun nasıl göründüğünü gör
   - Junior'ların yaptığı hataları öğren

**💻 Kod Örnekleri (4 saat):**
1. `samples/01-Beginner/PolymorphismBasics/01_SimplePolymorphism.cs`
   - Çalıştır, debug et, satır satır anla
   - `MakeSound()` metodunu nasıl override ediyor?
   
2. `samples/01-Beginner/PolymorphismBasics/02_AbstractClasses.cs`
   - Abstract class nedir?
   - Ne zaman kullanmalı?

3. `samples/01-Beginner/PolymorphismBasics/03_InterfacePolymorphism.cs`
   - Interface vs abstract class farkı
   - Hangi durumda hangisi?

**🧪 Alıştırmalar (5 saat):**
1. **Alıştırma 1:** `samples/99-Exercises/Polymorphism/01-ShapeCalculator/`
   - Görev: Circle, Rectangle, Triangle için `CalculateArea()` yaz
   - Polymorphism kullanarak
   - Test: `dotnet test` ile doğrula

2. **Alıştırma 2:** `samples/99-Exercises/Polymorphism/02-VehicleSystem/`
   - Görev: Car, Motorcycle, Truck için `CalculateFuel()` yaz
   - Abstract base class kullan
   - Test: Tüm testler yeşil olmalı

**🎯 Mini Proje (3 saat):**
```
Proje: Hayvanat Bahçesi Yönetim Sistemi
Dosya: YOL1_Hafta1_ZooProject/

Gereksinimler:
- Animal base class (abstract)
- Dog, Cat, Bird, Fish child class'ları
- Her hayvanın:
  - MakeSound() metodu (polymorphic)
  - Eat() metodu (polymorphic)
  - Move() metodu (polymorphic)
- Zoo class'ı:
  - AddAnimal() metodu
  - FeedAllAnimals() metodu (döngü ile tüm hayvanları besle)
  - MakeAllSounds() metodu

Hedef: if/else kullanmadan polymorphism ile çöz!
```

**✅ Hafta Sonu Başarı Ölçütleri:**
- [ ] Polymorphism'i kendi cümlelerinle açıklayabiliyorsun
- [ ] Virtual, override, abstract kavramlarını ayırt edebiliyorsun
- [ ] Zoo projesi çalışıyor ve testler geçiyor
- [ ] Kendi başına yeni hayvan ekleyebiliyorsun (örnek: Snake)

**📊 Zorluk:** ⭐⭐☆☆☆ (Orta)  
**📈 Tahmini İlerleme:** %4 (24 haftanın 1'i)

---

#### HAFTA 2: Upcasting, Downcasting ve Type Safety

**🎯 Bu Hafta Ne Öğreneceksin:**
- Upcasting (güvenli, implicit)
- Downcasting (riskli, explicit)
- `is`, `as` pattern matching
- InvalidCastException'dan kaçınma

**📖 Okuma (2 saat):**
1. `samples/01-Beginner/CastingExamples/WHY_THIS_PATTERN.md`
2. `samples/01-Beginner/Upcasting-Downcasting/` tüm dosyalar

**💻 Kod Örnekleri (3 saat):**
1. `samples/01-Beginner/Upcast-Downcast/Vehicle.cs`
   - Upcast örneği: `Vehicle vehicle = new Car();`
   - Neden her zaman güvenli?

2. `samples/01-Beginner/Upcast-Downcast/Car.cs`
   - Downcast örneği: `Car car = (Car)vehicle;`
   - Ne zaman InvalidCastException fırlatır?

3. `samples/01-Beginner/CastingExamples/SafeCasting.cs`
   - `is` kullanımı
   - `as` kullanımı
   - Pattern matching (C# 7+)

**🧪 Alıştırmalar (6 saat):**
1. **Exercise:** `samples/99-Exercises/Casting/01-EmployeeSystem/`
   - Employee, Manager, Developer hierarchy
   - Upcasting ile polymorphic collection
   - Downcasting ile specific özellikler
   
2. **Exercise:** `samples/99-Exercises/Casting/02-PaymentProcessor/`
   - IPaymentMethod interface
   - CreditCard, PayPal, Bitcoin implementations
   - Safe casting with `is` and `as`

**🎯 Mini Proje (4 saat):**
```
Proje: Şirket Bordro Sistemi
Dosya: YOL1_Hafta2_PayrollProject/

Gereksinimler:
- Employee base class
  - CalculateSalary() abstract
- FullTimeEmployee (sabit maaş)
- HourlyEmployee (saat × ücret)
- ContractorEmployee (proje bazlı)

Company class:
- List<Employee> employees
- AddEmployee() 
- CalculateTotalPayroll() → toplam maaş
- GetManagers() → sadece manager'ları döndür (downcast kullan!)
- GiveRaiseToFullTime(decimal percentage) → sadece full-time'a zam

Zorluk: Type-safe casting kullan, InvalidCastException fırlatma!
```

**✅ Hafta Sonu Kontrol:**
- [ ] Upcasting vs downcasting farkını biliyorsun
- [ ] `is` ve `as` operatörlerini kullanabiliyorsun
- [ ] InvalidCastException ne zaman olur biliyorsun
- [ ] Bordro projesi hatasız çalışıyor

**📊 Zorluk:** ⭐⭐☆☆☆  
**📈 İlerleme:** %8

---

#### HAFTA 3: LINQ Temelleri

**🎯 Bu Hafta Ne Öğreneceksin:**
- LINQ syntax (query ve method)
- Where, Select, OrderBy
- FirstOrDefault, Any, All
- LINQ to Objects

**📖 Okuma (2 saat):**
1. `samples/02-Intermediate/LINQ-Basics/WHY_THIS_PATTERN.md`
2. Microsoft LINQ documentation (link repo'da)

**💻 Kod Örnekleri (4 saat):**
1. `samples/02-Intermediate/LINQ-Basics/01_BasicQueries.cs`
2. `samples/02-Intermediate/LINQ-Basics/02_Filtering.cs`
3. `samples/02-Intermediate/LINQ-Basics/03_Projections.cs`

**🧪 Alıştırmalar (7 saat):**
1. `samples/99-Exercises/LINQ/01-BasicQueries/` (TÜM tasklar)
   - 10 LINQ sorgusu yaz
   - Her biri için test geçmeli

2. `samples/99-Exercises/LINQ/02-ProductCatalog/`
   - E-ticaret product filtreleme
   - Fiyat, kategori, stok bazlı sorgular

**🎯 Mini Proje (2 saat):**
```
Proje: Öğrenci Yönetim Sistemi
- Student listesi (Name, Grade, Age, Department)
- LINQ ile:
  - En başarılı 10 öğrenci
  - Departmana göre ortalama not
  - Başarısız öğrenciler (Grade < 50)
  - Yaş grubuna göre gruplama
```

**✅ Kontrol:**
- [ ] LINQ sorgusu yazabiliyorsun
- [ ] Method syntax vs query syntax biliyorsun
- [ ] Where, Select, OrderBy kullanabiliyorsun

**📊 Zorluk:** ⭐⭐☆☆☆  
**📈 İlerleme:** %12

---

#### HAFTA 4-6: OOP Derinlemesine (Details in LEARNING_PATHS.md)

**Özet:** Encapsulation, Inheritance, Composition, Interface Segregation

---

#### HAFTA 7-9: ASP.NET Core Basics (Details in LEARNING_PATHS.md)

**Özet:** Controller, Routing, Dependency Injection, API basics

---

#### HAFTA 10-12: Entity Framework + Database (Details in LEARNING_PATHS.md)

**Özet:** ORM temelleri, CRUD, migrations, relationships

---

#### HAFTA 13-16: CAPSTONE PROJECT - Blog Platform

**Gereksinimler:**
- ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- REST API endpoints (CRUD)
- Authentication (JWT - basit)
- Unit tests (xUnit)

**Tam detay:** `docs/LEARNING_PATHS.md` içinde "Path 1 Capstone" bölümüne bakın.

---

### 🎓 Yol 1 Mezuniyet Kriterleri

**Şunları yapabiliyorsan Junior Developer'sın:**

**✅ Teknik:**
- [ ] Polymorphism kullanarak genişletilebilir kod yazabiliyorsun
- [ ] LINQ ile veri sorgulama yapabiliyorsun
- [ ] Basit REST API geliştirebiliyorsun
- [ ] Entity Framework ile database işlemleri yapabiliyorsun
- [ ] Unit test yazabiliyorsun

**✅ Pratik:**
- [ ] Blog platformu capstone projeni tamamladın
- [ ] GitHub'da 3+ proje var
- [ ] README.md yazabiliyorsun
- [ ] Git ile versiyon kontrol yapabiliyorsun

**✅ Mülakat:**
- [ ] "Polymorphism nedir?" sorusunu cevaplayabiliyorsun
- [ ] "REST API nedir?" sorusunu cevaplayabiliyorsun
- [ ] FizzBuzz benzeri coding challenge çözebiliyorsun

**🎉 Mezun olduysan:**
→ YOL 2'ye geç (Junior → Mid-Level)  
→ VEYA iş aramaya başla + YOL 4 (Mülakat Prep) yap

---

## 🔥 YOL 2: JUNIOR'DAN MID-LEVEL'A

**👤 Hedef Kitle:**
- 6-12 ay C# deneyimi olan developer'lar
- Yol 1'i tamamlayanlar
- Basit CRUD uygulamaları yazabilenler

**📋 Ön Koşullar:**
- OOP'yi anlıyorsun (polymorphism, inheritance)
- Basit REST API yazabiliyorsun
- Entity Framework kullanabiliyorsun
- Git biliyorsun

**⏱️ Süre:** 6-9 ay (haftada 8-12 saat)

**🎯 Çıktı:**
- Mid-Level .NET Developer pozisyonu
- Design patterns uygulayabilme
- Mikroservis mimarisi anlama
- Performance optimization yapabilme
- Mid-level teknik mülakatlara hazır olma

**💰 Hedef Maaş:** $90,000-$120,000/yıl (ABD) veya ₺70,000-₺100,000/ay (Türkiye)

---

### 📅 HAFTA HAFTA DETAYLI PLAN (Yol 2)

#### HAFTA 1-2: Design Patterns Giriş - Factory & Strategy

**🎯 Hedef:**
- Factory Pattern'i anlama ve uygulama
- Strategy Pattern ile polymorphism bağlantısını görme
- Ne zaman hangi pattern kullanılacağını bilme

**📖 Okuma (3 saat):**
1. `samples/03-Advanced/DesignPatterns/WHY_THIS_PATTERN.md`
   - Factory ne problemi çözüyor?
   - Strategy ne zaman kullanılmalı?
   - Alternatives ve trade-offs
   
2. `docs/code-reviews/02-API-Design-Review/` 
   - Production'da pattern usage örnekleri

**💻 Kod Örnekleri (5 saat):**
1. `samples/03-Advanced/DesignPatterns/FactoryPattern.cs`
   - PaymentProcessorFactory örneği
   - Ne zaman factory, ne zaman direkt instantiation?

2. `samples/03-Advanced/DesignPatterns/StrategyPattern.cs`
   - Shipping calculator örneği
   - Strategy vs if/else farkı

3. `samples/03-Advanced/DesignPatterns/BuilderPattern.cs`
   - Complex object construction
   - Fluent interface pattern

**🧪 Alıştırmalar (8 saat):**
1. **Exercise:** `samples/99-Exercises/DesignPatterns/01-PaymentGateway/`
   - Stripe, PayPal, Cryptocurrency için factory
   - Her ödeme tipi için strategy implementation
   - Unit tests yaz

2. **Exercise:** `samples/99-Exercises/DesignPatterns/02-NotificationSystem/`
   - Email, SMS, Push için factory
   - Notification strategy'leri
   - Retry policy ekle (decorator pattern teaser)

**🎯 Gerçek Dünya Problemi (8 saat):**
```
Problem: samples/98-RealWorld-Problems/01-API-Rate-Limiting/

Görev:
1. PROBLEM.md'yi oku
2. Kendi çözümünü yaz (2 saat)
3. SOLUTION-A.md ile karşılaştır
4. SOLUTION-B.md'yi incele (Redis approach)
5. COMPARISON.md'den hangisini ne zaman kullanacağını öğren

Factory Pattern Uygulaması:
- RateLimiterFactory
- InMemoryRateLimiter strategy
- RedisRateLimiter strategy
- ApiGatewayRateLimiter strategy

Bu senaryo GERÇEK production problem!
```

**✅ 2 Hafta Sonu Kontrol:**
- [ ] Factory pattern ile object creation yapabiliyorsun
- [ ] Strategy pattern ile algorithm değiştirebiiyorsun
- [ ] Rate limiting problemini çözdün
- [ ] 3 farklı yaklaşımın trade-off'larını anlıyorsun

**📊 Zorluk:** ⭐⭐⭐☆☆  
**📈 İlerleme:** %7 (28 haftanın 2'si)

---

#### HAFTA 3-4: N+1 Query Problem & Database Optimization

**🎯 Hedef:**
- N+1 query nedir, nasıl tespit edilir?
- Eager loading vs lazy loading
- Query performance optimization

**📖 Okuma (2 saat):**
1. `samples/98-RealWorld-Problems/03-N-Plus-One-Problem/PROBLEM.md`
   - 1,527 sorgu problemi nasıl oluştu?
   - Production impact neydi?

**💻 Kod Örnekleri (4 saat):**
1. `samples/98-RealWorld-Problems/03-N-Plus-One-Problem/SOLUTION-A.md`
   - Include/ThenInclude kullanımı
   - AsNoTracking ne zaman?

2. `samples/03-Advanced/PerformanceOptimization/DatabaseOptimization.cs`
   - Query performance best practices

**🧪 Pratik (10 saat):**
```
Senaryo: E-Ticaret Order Sistemi

Verilen:
- Orders tablosu (500 satır)
- Order → Customer ilişkisi
- Order → OrderItems ilişkisi
- OrderItem → Product ilişkisi

Problem:
Şu anki kod 1,527 sorgu atıyor!

foreach (var order in orders)
{
    order.Customer = db.Customers.Find(order.CustomerId); // N queries
    foreach (var item in order.Items)
    {
        item.Product = db.Products.Find(item.ProductId); // N*M queries
    }
}

Görevler:
1. Sorunu tespit et (profiler kullan)
2. Eager loading ile düzelt
3. Benchmark yap (before/after)
4. Projection ile daha da optimize et

Hedef: 1,527 sorgu → 1 sorgu
```

**✅ Kontrol:**
- [ ] N+1 query problemini tespit edebiliyorsun
- [ ] Include ile fix yapabiliyorsun
- [ ] Before/after performance farkını gördün

**📊 Zorluk:** ⭐⭐⭐⭐☆  
**📈 İlerleme:** %14

---

#### HAFTA 5-6: Caching Strategies

**Gerçek Problem:** `samples/98-RealWorld-Problems/02-Cache-Strategy/`

**3 Çözüm Öğren:**
1. Cache-Aside (lazy loading)
2. Write-Through (strong consistency)
3. Hybrid (best of both)

**Uygulama:**
- Product catalog cache implementation
- Redis integration
- Cache invalidation strategies

**Detay:** `docs/LEARNING_PATHS.md` Yol 2, Hafta 5-6

---

#### HAFTA 7-24: Devam Eden Konular

**Hafta 7-9:** Microservice Error Handling  
**Hafta 10-12:** SOLID Principles Deep Dive  
**Hafta 13-15:** Performance Optimization (Span<T>, Memory<T>)  
**Hafta 16-18:** Advanced Testing (Integration, E2E)  
**Hafta 19-21:** CI/CD & DevOps Basics  
**Hafta 22-24:** Capstone - Microservice E-Commerce

**Tam detay:** `docs/LEARNING_PATHS.md` içinde

---

### 🎓 Yol 2 Mezuniyet Kriterleri

**Şunları yapabiliyorsan Mid-Level Developer'sın:**

**✅ Teknik:**
- [ ] 5+ design pattern uygulayabiliyorsun (Factory, Strategy, Observer, Decorator, Builder)
- [ ] N+1 query problemini tespit ve çözebiliyorsun
- [ ] Caching strategy tasarlayabiliyorsun
- [ ] Mikroservis mimarisini anlıyorsun
- [ ] Performance optimization yapabiliyorsun

**✅ Pratik:**
- [ ] 3 real-world problemi çözdün
- [ ] Mikroservis capstone'u tamamladın
- [ ] GitHub'da production-quality kod var

**✅ Mülakat:**
- [ ] System design soruları cevaplayabiliyorsun
- [ ] "Design a rate limiter" gibi sorulara yaklaşabiliyorsun
- [ ] Trade-off analizi yapabiliyorsun

**🎉 Mezun olduysan:**
→ YOL 3'e geç (Mid → Senior)  
→ VEYA mid-level pozisyonlara başvur + YOL 4 yap

---

## 🚀 YOL 3: MID-LEVEL'DAN SENIOR'A

**👤 Hedef Kitle:**
- 2-4 yıl C# deneyimi
- Design patterns biliyor
- Mikroservis deneyimi var

**⏱️ Süre:** 9-12 ay (haftada 6-10 saat)

**🎯 Çıktı:**
- Senior .NET Developer pozisyonu
- Sistem mimarisi tasarlayabilme
- Takım liderliği yapabilme
- Mentorship verebilme

**💰 Hedef Maaş:** $130,000-$180,000/yıl (ABD) veya ₺120,000-₺180,000/ay (Türkiye)

---

### 📅 YOL 3 ÖZET (Detay LEARNING_PATHS.md'de)

**Ay 1-3: Sistem Mimarisi**
- Distributed systems patterns
- Event-driven architecture
- CQRS, Event Sourcing

**Ay 4-6: Performance & Scalability**
- High-performance C# (Span<T>, stackalloc)
- Profiling ve optimization
- Load testing, stress testing

**Ay 7-9: Leadership & Mentorship**
- Code review leadership (docs/mentorship/ kullan)
- Junior mentoring
- Technical decision making

**Ay 10-12: Capstone - Enterprise System**
- Full-stack distributed system
- High availability, fault tolerance
- Production deployment

**Detay:** `docs/LEARNING_PATHS.md` Yol 3

---

## 💼 YOL 4: MÜLAKATA HAZIRLIK

**👤 Hedef Kitle:** 
- İş görüşmesine hazırlanan HER seviye
- Yol 1, 2 veya 3'ü tamamlayanlar

**⏱️ Süre:** 4-8 hafta (yoğun çalışma)

**🎯 Çıktı:** Interview-ready, özgüvenli, hazır

---

### 📅 4 HAFTALIK YOĞUN PROGRAM

#### HAFTA 1: Temel Kavramlar Revision

**Her Gün (2 saat):**
- Polymorphism, SOLID, Design Patterns revision
- `samples/01-Beginner/` ve `samples/03-Advanced/` hızlı geçiş
- CAREER_IMPACT.md dosyalarındaki mülakat sorularını çöz

**Yapılacaklar:**
- [ ] Tüm WHY_THIS_PATTERN.md'leri tekrar oku
- [ ] Her pattern için 1 cümle özet çıkar
- [ ] Whiteboard'da UML çizme pratiği yap

---

#### HAFTA 2: System Design Practice

**Her Gün (3 saat):**
- `samples/98-RealWorld-Problems/` her problemi whiteboard'da çöz
- Decision tree'leri ezberle
- Trade-off analysis pratiği

**Mock Interview Practice:**
```
Sorular:
1. "Design a rate limiter" (samples/98-RealWorld-Problems/01)
2. "Design a cache" (samples/98-RealWorld-Problems/02)
3. "Why are queries slow?" (samples/98-RealWorld-Problems/03)

Her birini:
- 5 dakika: Problem anla
- 10 dakika: High-level design çiz
- 15 dakika: Deep dive (trade-offs discuss)
- 5 dakika: Questions & clarifications
```

---

#### HAFTA 3: Code Review Simülasyonu

**Her Gün (2 saat):**
- `docs/code-reviews/` tüm senaryoları çalış
- `docs/mentorship/common-junior-mistakes.md` oku
- Kötü kodu görünce ne söylersin? Practice yap

**Yapılacaklar:**
- [ ] 5 bad code örneği bul (GitHub'dan)
- [ ] Her biri için review comment yaz
- [ ] `docs/mentorship/code-review-checklist.md` ile karşılaştır

---

#### HAFTA 4: Behavioral + Final Prep

**Her Gün (2-3 saat):**
- CAREER_IMPACT.md'lerden "Production Problems" senaryolarını hikaye olarak hazırla
- "Tell me about a time..." sorularına cevap hazırla
- GitHub portfolio'nu gözden geçir

**Mock Interview (Kendine veya Arkadaşınla):**
1. 45 dakika: Technical (coding + system design)
2. 30 dakika: Behavioral questions
3. 15 dakika: Questions for interviewer

**Detay:** `docs/LEARNING_PATHS.md` Yol 4

---

## 📊 İLERLEME TAKİBİ

### Nasıl Takip Ederim?

**Haftalık:**
```markdown
## Hafta X Raporu

✅ Tamamlanan:
- [ ] Sample/Exercise adı
- [ ] Mini proje

⏳ Devam Edenler:
- [ ] Zorluk çektiğim konu

📝 Notlar:
- Ne öğrendim?
- Nerede takıldım?
- Yardıma ihtiyacım var mı?
```

**Aylık:**
```markdown
## Ay X Değerlendirme

Teknik Seviye (1-5):
- Polymorphism: [X/5]
- Design Patterns: [X/5]
- Performance: [X/5]

Hedefler:
- [ ] Bu ay tamamlanan
- [ ] Gelecek ay hedefi
```

---

## 🆘 SIKÇA SORULAN SORULAR

**S: "Programlama bilmiyorum, buradan başlayabilir miyim?"**
C: HAYIR. Önce temel programlama öğren (değişken, döngü, fonksiyon). Sonra Yol 1'e başla.

**S: "3 yıl Python biliyorum, C# bilmiyorum. Nereden başlamalıyım?"**
C: Yol 1'in ilk 4 haftasını hızlı geç (C# syntax). Sonra Yol 2'ye atla.

**S: "5 yıl C# biliyorum ama design pattern bilmiyorum. Hangisi?"**
C: Yol 2. Design patterns senior olmak için şart.

**S: "Her hafta 10 saat ayıramıyorum, ne yapmalıyım?"**
C: Süreyi 2x uzat. Mesela Yol 1 → 6 ay yerine 12 ay. Önemli olan düzenli çalışmak.

**S: "Capstone projesinde takıldım, yardım?"**
C: GitHub Issues'a yaz veya docs/mentorship/common-junior-mistakes.md'ye bak.

**S: "Tüm 4 yolu tamamlarsam ne olur?"**
C: Senior .NET Developer olursun! ~2-3 yıl sürer ama her yol seni bir üst levele çıkarır.

---

## 🎯 SONRAKİ ADIM

**ŞİMDİ YAP:**

1. **Seviyeni belirle** (yukarıdaki sorulara cevap ver)
2. **Yolunu seç** (Yol 1, 2, 3 veya 4)
3. **İlk dosyayı aç** (bu README'nin başındaki "İlk 3 Dosya" bölümü)
4. **İlk saatini başlat** (60 dakika, şimdi!)

**Başlamak için:**
```bash
# Yol 1 → Junior için:
cd samples/01-Beginner/PolymorphismBasics/
code WHY_THIS_PATTERN.md

# Yol 2 → Mid için:
cd samples/03-Advanced/DesignPatterns/
code WHY_THIS_PATTERN.md

# Yol 3 → Senior için:
cd samples/98-RealWorld-Problems/01-API-Rate-Limiting/
code PROBLEM.md

# Yol 4 → Mülakat için:
cd docs/code-reviews/01-Polymorphism-Review/
code review-comments.md
```

---

## 📚 EK KAYNAKLAR

**Tam Detay İçin:**
- `docs/LEARNING_PATHS.md` - 1,885 satır hafta hafta plan
- `docs/mentorship/growth-plan-template.md` - Kişisel growth plan
- Her sample'ın `WHY_THIS_PATTERN.md` - Derinlemesine açıklama
- Her sample'ın `CAREER_IMPACT.md` - CV ve mülakat rehberi

**Community:**
- GitHub Issues: Soru sor
- GitHub Discussions: Diğer öğrencilerle konuş
- Pull Requests: Kendi çözümlerini paylaş

---

**🎉 BAŞARILAR! İlk adımı atmak en zoruydu. Şimdi git ve YAP! 🚀**

