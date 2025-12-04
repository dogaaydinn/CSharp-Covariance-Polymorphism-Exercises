# Why Observer Pattern?

## Problem
Subject, observer'ları direkt biliyorsa tight coupling.

## Çözüm
Interface ile loose coupling.

## C# Implementation
Modern C#'ta Event kullan:
```csharp
public event EventHandler<PriceChangedEventArgs> PriceChanged;
```

## Gerçek Dünya
- INotifyPropertyChanged (WPF/MAUI)
- Event aggregators (Prism, MediatR)
- Reactive Extensions (Rx.NET)
