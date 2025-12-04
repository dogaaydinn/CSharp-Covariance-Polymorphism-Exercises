# Neden Variance?

## ❌ Without Variance
```csharp
interface IProducer<T> {  // Invariant
    T Produce();
}
IProducer<Dog> producer = ...;
IProducer<Animal> animal = producer;  // ❌ Error!
```

## ✅ With Covariance
```csharp
interface IProducer<out T> {  // Covariant
    T Produce();
}
IProducer<Dog> producer = ...;
IProducer<Animal> animal = producer;  // ✅ OK
```

## ✨ Faydalar
1. **Type Safety**: Compile-time güvenlik
2. **Flexibility**: Type conversions
3. **Real-world**: Collection interfaces (IEnumerable<out T>)
