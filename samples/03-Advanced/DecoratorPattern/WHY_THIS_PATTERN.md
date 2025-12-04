# Why Decorator?

Inheritance yerine composition.
Runtime'da sorumluluklarekle.
Open/Closed principle.

## .NET Examples
```csharp
Stream file = new FileStream("data.txt");
Stream buffered = new BufferedStream(file);
Stream compressed = new GZipStream(buffered);
```
