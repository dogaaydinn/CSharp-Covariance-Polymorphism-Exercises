namespace SOLIDPrinciples.InterfaceSegregation;

// ❌ BAD: Fat interface - forces implementations of unused methods
public interface IBadPrinter
{
    void Print(string document);
    void Scan(string document);
    void Fax(string document);
}

public class BasicPrinter : IBadPrinter
{
    public void Print(string document)
    {
        Console.WriteLine($"Printing: {document}");
    }

    public void Scan(string document)
    {
        throw new NotImplementedException("Basic printer can't scan!");
    }

    public void Fax(string document)
    {
        throw new NotImplementedException("Basic printer can't fax!");
    }
}

// ✅ GOOD: Segregated interfaces - clients use only what they need
public interface IPrinter
{
    void Print(string document);
}

public interface IScanner
{
    void Scan(string document);
}

public interface IFax
{
    void Fax(string document);
}

// Multifunction device implements all
public interface IMultifunctionDevice : IPrinter, IScanner, IFax
{
}

public class SimplePrinter : IPrinter
{
    public void Print(string document)
    {
        Console.WriteLine($"✅ Printing: {document}");
    }
}

public class MultifunctionPrinter : IMultifunctionDevice
{
    public void Print(string document)
    {
        Console.WriteLine($"✅ MFP Printing: {document}");
    }

    public void Scan(string document)
    {
        Console.WriteLine($"✅ MFP Scanning: {document}");
    }

    public void Fax(string document)
    {
        Console.WriteLine($"✅ MFP Faxing: {document}");
    }
}

// WHY?
// - No class is forced to implement methods it doesn't need
// - SimplePrinter only implements IPrinter
// - Multifunction devices implement all interfaces
// - More flexible and maintainable
