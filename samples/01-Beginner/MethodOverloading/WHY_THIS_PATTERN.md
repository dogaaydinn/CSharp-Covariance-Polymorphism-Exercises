# Neden Method Overloading?

## 🤔 Problem
Aynı işlevi farklı parametrelerle yapmak istiyoruz.

## ❌ Kötü Yaklaşım
```csharp
int AddTwoNumbers(int a, int b) { }
int AddThreeNumbers(int a, int b, int c) { }
double AddTwoDoubles(double a, double b) { }
// Her varyant için farklı isim - karmaşık!
```

## ✅ İyi Yaklaşım
```csharp
int Add(int a, int b) { }
int Add(int a, int b, int c) { }
double Add(double a, double b) { }
// Aynı isim, farklı parametreler - temiz!
```

## ✨ Faydalar
1. **Okunabilirlik**: Aynı işlev, aynı isim
2. **Intuitive API**: Kullanıcı doğal olarak bulur
3. **Compile-time safety**: Tip kontrolü
4. **IntelliSense**: IDE desteği

## 🎯 Ne Zaman Kullanmalı?
- Aynı işlev, farklı input kombinasyonları
- Convenience overloads (wrapper)
- Default values sağlamak için
- API'yi esnek yapmak için
