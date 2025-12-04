# Neden Constructor Chaining?

## ❌ Kötü: Kod Tekrarı
```csharp
public Person(string name) {
    Name = name;
    Age = 0;  // Tekrar!
    Address = "Unknown";  // Tekrar!
}

public Person(string name, int age) {
    Name = name;
    Age = age;
    Address = "Unknown";  // Tekrar!
}
```

## ✅ İyi: Constructor Chaining
```csharp
public Person() {
    Age = 0;
    Address = "Unknown";
}

public Person(string name) : this() {
    Name = name;
}

public Person(string name, int age) : this(name) {
    Age = age;
}
```

## ✨ Faydalar
1. **DRY Principle**: Don't Repeat Yourself
2. **Maintainability**: Tek yerden değiştir
3. **Consistency**: Tüm constructor'lar aynı mantığı kullanır
4. **Clear Intent**: Initialization flow açık
