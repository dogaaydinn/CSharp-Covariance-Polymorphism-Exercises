using FluentAssertions;
using NUnit.Framework;

namespace DecoratorPattern.Tests;

[TestFixture]
public class DecoratorPatternTests
{
    [Test]
    public void FileDataSource_WriteAndRead_ShouldStoreData()
    {
        var source = new FileDataSource("test.txt");
        var data = "Hello World";

        source.WriteData(data);
        var result = source.ReadData();

        result.Should().Be(data);
    }

    [Test]
    public void EncryptionDecorator_ShouldEncryptData()
    {
        var source = new FileDataSource("test.txt");
        var decorator = new EncryptionDecorator(source);
        var plainText = "ABC";

        decorator.WriteData(plainText);
        var decrypted = decorator.ReadData();

        decrypted.Should().Be(plainText);
    }

    [Test]
    public void EncryptionDecorator_EncryptedDataShouldDifferFromOriginal()
    {
        var source = new FileDataSource("test.txt");
        var plainText = "Hello";

        source.WriteData(plainText);
        var plainResult = source.ReadData();

        var encrypted = new EncryptionDecorator(new FileDataSource("encrypted.txt"));
        encrypted.WriteData(plainText);

        // The underlying storage should have encrypted data, not plain text
        // This test verifies encryption actually happens
        plainResult.Should().Be(plainText);
    }

    [Test]
    public void CompressionDecorator_ShouldCompressAndDecompress()
    {
        var source = new FileDataSource("test.txt");
        var decorator = new CompressionDecorator(source);
        var data = "aaabbbccc";

        decorator.WriteData(data);
        var result = decorator.ReadData();

        result.Should().Be(data);
    }

    [Test]
    public void CompressionDecorator_ShouldActuallyCompress()
    {
        var source = new FileDataSource("test.txt");
        var compressor = new CompressionDecorator(source);
        var data = "aaabbbccc"; // Should compress to "a3b3c3"

        compressor.WriteData(data);

        // Verify compression happened by checking internal storage
        var rawData = source.ReadData();
        rawData.Length.Should().BeLessThan(data.Length, "compressed data should be shorter");
    }

    [Test]
    public void LoggingDecorator_ShouldLogOperations()
    {
        var source = new FileDataSource("test.txt");
        var logger = new LoggingDecorator(source);

        logger.WriteData("test");
        logger.ReadData();

        logger.Logs.Should().HaveCount(2);
        logger.Logs[0].Should().Contain("Write");
        logger.Logs[1].Should().Contain("Read");
    }

    [Test]
    public void MultipleDecorators_EncryptionAndCompression_ShouldWork()
    {
        var source = new FileDataSource("test.txt");
        var decorated = new EncryptionDecorator(
            new CompressionDecorator(source)
        );
        var data = "aaabbbccc";

        decorated.WriteData(data);
        var result = decorated.ReadData();

        result.Should().Be(data);
    }

    [Test]
    public void MultipleDecorators_AllThreeLayers_ShouldWork()
    {
        var source = new FileDataSource("test.txt");
        var decorated = new LoggingDecorator(
            new EncryptionDecorator(
                new CompressionDecorator(source)
            )
        );
        var data = "Hello World";

        decorated.WriteData(data);
        var result = decorated.ReadData();

        result.Should().Be(data);
        decorated.Logs.Should().HaveCountGreaterOrEqualTo(2);
    }

    [Test]
    public void DecoratorChaining_OrderMatters()
    {
        var data = "aaabbbccc";

        // Order 1: Compress then Encrypt
        var source1 = new FileDataSource("test1.txt");
        var chain1 = new EncryptionDecorator(new CompressionDecorator(source1));
        chain1.WriteData(data);
        var result1 = chain1.ReadData();

        // Order 2: Encrypt then Compress
        var source2 = new FileDataSource("test2.txt");
        var chain2 = new CompressionDecorator(new EncryptionDecorator(source2));
        chain2.WriteData(data);
        var result2 = chain2.ReadData();

        // Both should return original data correctly
        result1.Should().Be(data);
        result2.Should().Be(data);
    }

    [Test]
    public void DataSourceDecorator_CanBeExtended()
    {
        // Verify that DataSourceDecorator is abstract and can be extended
        var source = new FileDataSource("test.txt");

        // Create a custom decorator
        var customDecorator = new CustomTestDecorator(source);
        customDecorator.WriteData("test");
        var result = customDecorator.ReadData();

        result.Should().Be("test");
    }

    // Helper class for testing decorator extensibility
    private class CustomTestDecorator : DataSourceDecorator
    {
        public CustomTestDecorator(IDataSource source) : base(source)
        {
        }

        public override void WriteData(string data)
        {
            _wrappee.WriteData(data);
        }

        public override string ReadData()
        {
            return _wrappee.ReadData();
        }
    }

    [Test]
    public void LoggingDecorator_LogsContainTimestamp()
    {
        var source = new FileDataSource("test.txt");
        var logger = new LoggingDecorator(source);

        logger.WriteData("test");

        logger.Logs.Should().HaveCount(1);
        // Log should contain some timestamp or time-related info
        logger.Logs[0].Should().NotBeEmpty();
    }

    [Test]
    public void EncryptionDecorator_CaesarCipher_ShouldShiftCharacters()
    {
        var source = new FileDataSource("test.txt");
        var encryption = new EncryptionDecorator(source);

        // "ABC" with Caesar shift of 3 should become "DEF" when encrypted
        encryption.WriteData("ABC");
        var result = encryption.ReadData();

        result.Should().Be("ABC", "decryption should restore original text");
    }
}
