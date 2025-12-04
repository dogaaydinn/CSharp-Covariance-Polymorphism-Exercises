namespace DecoratorPattern;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Decorator Pattern Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions\n");

        // TODO: Uncomment and test when implementation is complete
        /*
        // Example 1: Basic file data source
        IDataSource source = new FileDataSource("data.txt");
        source.WriteData("Hello World");
        Console.WriteLine($"Plain: {source.ReadData()}");

        // Example 2: File + Encryption
        IDataSource encrypted = new EncryptionDecorator(new FileDataSource("secure.txt"));
        encrypted.WriteData("Secret Message");
        Console.WriteLine($"Encrypted: {encrypted.ReadData()}");

        // Example 3: File + Compression
        IDataSource compressed = new CompressionDecorator(new FileDataSource("compressed.txt"));
        compressed.WriteData("aaabbbccc");
        Console.WriteLine($"Compressed: {compressed.ReadData()}");

        // Example 4: Multiple decorators - Encryption + Compression + Logging
        IDataSource fullyDecorated = new LoggingDecorator(
            new EncryptionDecorator(
                new CompressionDecorator(
                    new FileDataSource("full.txt")
                )
            )
        );
        fullyDecorated.WriteData("Important Data");
        Console.WriteLine($"Fully Decorated: {fullyDecorated.ReadData()}");

        if (fullyDecorated is LoggingDecorator logger)
        {
            Console.WriteLine("\nLogs:");
            logger.Logs.ForEach(Console.WriteLine);
        }
        */
    }
}
