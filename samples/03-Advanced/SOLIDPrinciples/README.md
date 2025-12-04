# SOLID Principles

**Konu**: Object-oriented design prensipler (S.O.L.I.D)

## Öğrenilen Kavramlar

- ✅ Single Responsibility Principle (SRP)
- ✅ Open/Closed Principle (OCP)
- ✅ Liskov Substitution Principle (LSP)
- ✅ Interface Segregation Principle (ISP)
- ✅ Dependency Inversion Principle (DIP)

## Kullanım

```bash
dotnet run
```

## Örnekler

### 1. Single Responsibility (SRP)
❌ Bad: `BadReportGenerator` - report oluşturma VE email gönderme
✅ Good: `ReportGenerator` + `EmailSender` - ayrı sorumluluklar

### 2. Open/Closed (OCP)
❌ Bad: `BadDiscountCalculator` - yeni indirim için class değişikliği
✅ Good: `IDiscountStrategy` - yeni class ekle, mevcut değiştirme

### 3. Liskov Substitution (LSP)
❌ Bad: `BadPenguin` - Fly() exception fırlatıyor
✅ Good: `ISwimmingBird`, `IFlyingBird` - doğru soyutlama

### 4. Interface Segregation (ISP)
❌ Bad: `IBadPrinter` - tüm cihazlar tüm metodları implement etmeli
✅ Good: `IPrinter`, `IScanner`, `IFax` - segregated interfaces

### 5. Dependency Inversion (DIP)
❌ Bad: `BadNotificationService` - concrete class'a bağımlı
✅ Good: `NotificationService(INotifier)` - abstraction'a bağımlı

## Çıktı

```
=== SOLID Principles Demo ===

1️⃣  SINGLE RESPONSIBILITY PRINCIPLE
❌ BAD: One class doing everything
✅ GOOD: Separated responsibilities

2️⃣  OPEN/CLOSED PRINCIPLE
❌ BAD: Modifying class for new discount types
✅ GOOD: Extending with new classes

...
```
