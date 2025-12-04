namespace DecoratorPattern;

// TODO 1: IDataSource - Component interface
public interface IDataSource
{
    void WriteData(string data);
    string ReadData();
}

// TODO 2: FileDataSource - Concrete component
public class FileDataSource : IDataSource
{
    private string _filename;
    private string? _data;

    public FileDataSource(string filename)
    {
        _filename = filename;
    }

    public void WriteData(string data)
    {
        // TODO: Store data (simulate file write)
        throw new NotImplementedException();
    }

    public string ReadData()
    {
        // TODO: Return stored data (simulate file read)
        throw new NotImplementedException();
    }
}

// TODO 3: DataSourceDecorator - Base decorator
public abstract class DataSourceDecorator : IDataSource
{
    protected IDataSource _wrappee;

    protected DataSourceDecorator(IDataSource source)
    {
        _wrappee = source;
    }

    public virtual void WriteData(string data)
    {
        // TODO: Delegate to wrappee
        throw new NotImplementedException();
    }

    public virtual string ReadData()
    {
        // TODO: Delegate to wrappee
        throw new NotImplementedException();
    }
}

// TODO 4: EncryptionDecorator - Adds encryption
public class EncryptionDecorator : DataSourceDecorator
{
    public EncryptionDecorator(IDataSource source) : base(source)
    {
    }

    public override void WriteData(string data)
    {
        // TODO: Encrypt data before writing
        // Use simple Caesar cipher: shift each char by 3
        throw new NotImplementedException();
    }

    public override string ReadData()
    {
        // TODO: Read and decrypt data
        // Decrypt by shifting back by 3
        throw new NotImplementedException();
    }

    private string Encrypt(string plainText)
    {
        // TODO: Implement Caesar cipher encryption
        throw new NotImplementedException();
    }

    private string Decrypt(string cipherText)
    {
        // TODO: Implement Caesar cipher decryption
        throw new NotImplementedException();
    }
}

// TODO 5: CompressionDecorator - Adds compression
public class CompressionDecorator : DataSourceDecorator
{
    public CompressionDecorator(IDataSource source) : base(source)
    {
    }

    public override void WriteData(string data)
    {
        // TODO: Compress data before writing
        // Simple compression: remove duplicate consecutive chars
        throw new NotImplementedException();
    }

    public override string ReadData()
    {
        // TODO: Read and decompress data
        throw new NotImplementedException();
    }

    private string Compress(string data)
    {
        // TODO: Simple compression - replace consecutive chars with count
        // Example: "aaabbc" -> "a3b2c1"
        throw new NotImplementedException();
    }

    private string Decompress(string data)
    {
        // TODO: Decompress the data
        // Example: "a3b2c1" -> "aaabbc"
        throw new NotImplementedException();
    }
}

// TODO 6: LoggingDecorator - Adds logging
public class LoggingDecorator : DataSourceDecorator
{
    public List<string> Logs { get; } = new();

    public LoggingDecorator(IDataSource source) : base(source)
    {
    }

    public override void WriteData(string data)
    {
        // TODO: Log the write operation and delegate
        throw new NotImplementedException();
    }

    public override string ReadData()
    {
        // TODO: Log the read operation and delegate
        throw new NotImplementedException();
    }

    private void Log(string message)
    {
        // TODO: Add timestamped log entry
        throw new NotImplementedException();
    }
}
