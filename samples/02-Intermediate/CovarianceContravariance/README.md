# Covariance & Contravariance

## 📚 Konu
Generic variance: covariance (out), contravariance (in), invariance.

## 🔑 Kavramlar
- **Covariance (out)**: T sadece return type - IProducer<Dog> → IProducer<Animal>
- **Contravariance (in)**: T sadece parameter - IConsumer<Animal> → IConsumer<Dog>
- **Invariance**: T hem input hem output - no conversion

## 💻 Kullanım
```bash
cd samples/02-Intermediate/CovarianceContravariance
dotnet run
```

## 🎓 Örnekler
```csharp
// Covariance
public interface IProducer<out T> {
    T Produce();
}
IProducer<Dog> → IProducer<Animal>  // ✅

// Contravariance
public interface IConsumer<in T> {
    void Consume(T item);
}
IConsumer<Animal> → IConsumer<Dog>  // ✅
```

## 💡 Rules
- **out**: T only in return positions
- **in**: T only in parameter positions
- Neither: Invariant
