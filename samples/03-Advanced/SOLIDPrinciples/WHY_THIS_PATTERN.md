# Why SOLID Principles?

## Problem
Kötü tasarlanmış kod:
- 🔴 Değişiklik yapmak zor
- 🔴 Test etmek zor
- 🔴 Yeniden kullanılamaz
- 🔴 Hataya açık

## SOLID Çözümü

### S - Single Responsibility
**Bir class'ın değişmek için sadece bir nedeni olmalı**
- Report generation ≠ Email sending
- Her class tek bir şey yapar, ama iyi yapar

### O - Open/Closed
**Extension'a açık, modification'a kapalı**
- Yeni özellik = Yeni class
- Mevcut kodu değiştirme

### L - Liskov Substitution
**Alt sınıflar üst sınıf yerine kullanılabilmeli**
- Penguin, Bird davranışını bozmamalı
- Contract'ı ihlal etme

### I - Interface Segregation
**Fat interfaces yerine specific interfaces**
- BasicPrinter, scan() implement etmek zorunda kalmamalı
- İhtiyacın olanı implement et

### D - Dependency Inversion
**Abstraction'a bağlan, implementation'a değil**
- INotifier kullan, EmailNotifier değil
- Dependency injection ile esneklik

## Faydalar
✅ Maintainable kod
✅ Test edilebilir
✅ Yeniden kullanılabilir
✅ Esnek ve genişletilebilir

## Gerçek Dünya
- ASP.NET Core: DI container (DIP)
- Entity Framework: Repository pattern (SRP, DIP)
- Payment gateways: Strategy pattern (OCP)
- Authorization: Interface segregation (ISP)
