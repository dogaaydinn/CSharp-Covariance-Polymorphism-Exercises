using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.Structural;

/// <summary>
/// ADAPTER PATTERN - Makes incompatible interfaces work together
///
/// Problem:
/// - Need to use existing class with incompatible interface
/// - Can't modify existing code (third-party libraries, legacy code)
/// - Want to integrate new code with old code
/// - Multiple classes need same interface but have different implementations
///
/// UML Structure:
/// ┌──────────────┐        ┌──────────────┐
/// │    Client    │───────>│    Target    │ (interface)
/// └──────────────┘        └──────────────┘
///                                △
///                                │ implements
///                         ┌──────┴──────┐
///                         │   Adapter   │
///                         └─────────────┘
///                                │ uses
///                                ▼
///                         ┌─────────────┐
///                         │  Adaptee    │ (existing class)
///                         └─────────────┘
///
/// When to Use:
/// - Integrate legacy code with new systems
/// - Use third-party libraries with different interfaces
/// - Make incompatible interfaces compatible
/// - Create reusable classes that cooperate with unrelated classes
///
/// Benefits:
/// - Single Responsibility Principle
/// - Open/Closed Principle
/// - Reuse existing functionality
/// - Integrate incompatible code
/// </summary>

#region Payment Gateway Example

/// <summary>
/// Target interface - What our system expects
/// </summary>
public interface IPaymentProcessor
{
    bool ProcessPayment(decimal amount, string currency, string accountId);
    string GetTransactionId();
    bool RefundPayment(string transactionId, decimal amount);
}

/// <summary>
/// Adaptee - Legacy payment system with different interface
/// </summary>
public class LegacyPaymentSystem
{
    private int _lastTransactionNumber = 1000;

    public bool MakePayment(double dollars, string account)
    {
        _lastTransactionNumber++;
        Console.WriteLine($"  [Adapter] Legacy System: Processing ${dollars:F2} from account {account}");
        Console.WriteLine($"  [Adapter] Legacy System: Transaction number {_lastTransactionNumber}");
        return true;
    }

    public int GetLastTransactionNumber() => _lastTransactionNumber;

    public bool ProcessRefund(int transactionNumber, double dollars)
    {
        Console.WriteLine($"  [Adapter] Legacy System: Refunding ${dollars:F2} for transaction {transactionNumber}");
        return true;
    }
}

/// <summary>
/// Adapter - Makes legacy system compatible with modern interface
/// </summary>
public class LegacyPaymentAdapter : IPaymentProcessor
{
    private readonly LegacyPaymentSystem _legacySystem;
    private string _lastTransactionId = string.Empty;

    public LegacyPaymentAdapter(LegacyPaymentSystem legacySystem)
    {
        _legacySystem = legacySystem ?? throw new ArgumentNullException(nameof(legacySystem));
    }

    public bool ProcessPayment(decimal amount, string currency, string accountId)
    {
        // Convert decimal to double (legacy system uses double)
        double dollars = (double)amount;

        // Legacy system only supports USD
        if (currency != "USD")
        {
            Console.WriteLine($"  [Adapter] Converting {currency} to USD");
            // Simulate currency conversion
            dollars *= GetConversionRate(currency);
        }

        // Use legacy system
        bool success = _legacySystem.MakePayment(dollars, accountId);

        // Convert transaction number to modern ID format
        if (success)
        {
            int transactionNum = _legacySystem.GetLastTransactionNumber();
            _lastTransactionId = $"TXN-{transactionNum:D6}";
        }

        return success;
    }

    public string GetTransactionId() => _lastTransactionId;

    public bool RefundPayment(string transactionId, decimal amount)
    {
        // Convert modern ID format back to legacy transaction number
        if (transactionId.StartsWith("TXN-"))
        {
            int transactionNum = int.Parse(transactionId.Substring(4));
            double dollars = (double)amount;
            return _legacySystem.ProcessRefund(transactionNum, dollars);
        }

        return false;
    }

    private double GetConversionRate(string currency) => currency switch
    {
        "EUR" => 1.10,
        "GBP" => 1.25,
        "JPY" => 0.0091,
        _ => 1.0
    };
}

/// <summary>
/// Modern payment system that already implements our interface
/// </summary>
public class StripePaymentProcessor : IPaymentProcessor
{
    private string _lastTransactionId = string.Empty;

    public bool ProcessPayment(decimal amount, string currency, string accountId)
    {
        _lastTransactionId = $"STRIPE-{Guid.NewGuid():N}";
        Console.WriteLine($"  [Adapter] Stripe: Processing {amount:F2} {currency} from {accountId}");
        Console.WriteLine($"  [Adapter] Stripe: Transaction ID {_lastTransactionId}");
        return true;
    }

    public string GetTransactionId() => _lastTransactionId;

    public bool RefundPayment(string transactionId, decimal amount)
    {
        Console.WriteLine($"  [Adapter] Stripe: Refunding {amount:F2} for {transactionId}");
        return true;
    }
}

#endregion

#region Media Player Example

/// <summary>
/// Target interface
/// </summary>
public interface IMediaPlayer
{
    void Play(string filename);
    void Stop();
    void SetVolume(int level);
}

/// <summary>
/// Adaptee - Third-party video player
/// </summary>
public class ThirdPartyVideoPlayer
{
    public void OpenVideoFile(string path)
    {
        Console.WriteLine($"  [Adapter] ThirdPartyVideoPlayer: Opening video file {path}");
    }

    public void StartPlayback()
    {
        Console.WriteLine("  [Adapter] ThirdPartyVideoPlayer: Starting video playback");
    }

    public void StopPlayback()
    {
        Console.WriteLine("  [Adapter] ThirdPartyVideoPlayer: Stopping video playback");
    }

    public void AdjustVolume(double volumeLevel) // 0.0 to 1.0
    {
        Console.WriteLine($"  [Adapter] ThirdPartyVideoPlayer: Volume set to {volumeLevel:P0}");
    }
}

/// <summary>
/// Adaptee - Third-party audio player
/// </summary>
public class ThirdPartyAudioPlayer
{
    private string _currentTrack = string.Empty;

    public void LoadTrack(string trackPath)
    {
        _currentTrack = trackPath;
        Console.WriteLine($"  [Adapter] ThirdPartyAudioPlayer: Loaded track {trackPath}");
    }

    public void PlayTrack()
    {
        Console.WriteLine($"  [Adapter] ThirdPartyAudioPlayer: Playing {_currentTrack}");
    }

    public void PauseTrack()
    {
        Console.WriteLine($"  [Adapter] ThirdPartyAudioPlayer: Paused {_currentTrack}");
    }

    public void SetVolumeLevel(int percentage) // 0 to 100
    {
        Console.WriteLine($"  [Adapter] ThirdPartyAudioPlayer: Volume {percentage}%");
    }
}

/// <summary>
/// Adapter for video player
/// </summary>
public class VideoPlayerAdapter : IMediaPlayer
{
    private readonly ThirdPartyVideoPlayer _videoPlayer;

    public VideoPlayerAdapter(ThirdPartyVideoPlayer videoPlayer)
    {
        _videoPlayer = videoPlayer ?? throw new ArgumentNullException(nameof(videoPlayer));
    }

    public void Play(string filename)
    {
        _videoPlayer.OpenVideoFile(filename);
        _videoPlayer.StartPlayback();
    }

    public void Stop()
    {
        _videoPlayer.StopPlayback();
    }

    public void SetVolume(int level)
    {
        // Convert 0-100 to 0.0-1.0
        double volumeLevel = level / 100.0;
        _videoPlayer.AdjustVolume(volumeLevel);
    }
}

/// <summary>
/// Adapter for audio player
/// </summary>
public class AudioPlayerAdapter : IMediaPlayer
{
    private readonly ThirdPartyAudioPlayer _audioPlayer;

    public AudioPlayerAdapter(ThirdPartyAudioPlayer audioPlayer)
    {
        _audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer));
    }

    public void Play(string filename)
    {
        _audioPlayer.LoadTrack(filename);
        _audioPlayer.PlayTrack();
    }

    public void Stop()
    {
        _audioPlayer.PauseTrack();
    }

    public void SetVolume(int level)
    {
        _audioPlayer.SetVolumeLevel(level);
    }
}

#endregion

#region Data Source Example

/// <summary>
/// Target interface - Our application's expected interface
/// </summary>
public interface IDataSource
{
    List<string> GetData();
    void SaveData(List<string> data);
}

/// <summary>
/// Adaptee - Legacy XML data provider
/// </summary>
public class XmlDataProvider
{
    public string ReadXml()
    {
        Console.WriteLine("  [Adapter] Reading data from XML format");
        return "<data><item>Record 1</item><item>Record 2</item><item>Record 3</item></data>";
    }

    public void WriteXml(string xmlData)
    {
        Console.WriteLine($"  [Adapter] Writing data to XML: {xmlData}");
    }
}

/// <summary>
/// Adapter for XML data provider
/// </summary>
public class XmlDataAdapter : IDataSource
{
    private readonly XmlDataProvider _xmlProvider;

    public XmlDataAdapter(XmlDataProvider xmlProvider)
    {
        _xmlProvider = xmlProvider ?? throw new ArgumentNullException(nameof(xmlProvider));
    }

    public List<string> GetData()
    {
        string xml = _xmlProvider.ReadXml();

        // Parse XML to List (simplified parsing)
        Console.WriteLine("  [Adapter] Converting XML to List<string>");
        var items = new List<string>();

        int startIndex = 0;
        while (true)
        {
            int itemStart = xml.IndexOf("<item>", startIndex);
            if (itemStart == -1) break;

            int itemEnd = xml.IndexOf("</item>", itemStart);
            if (itemEnd == -1) break;

            string item = xml.Substring(itemStart + 6, itemEnd - itemStart - 6);
            items.Add(item);
            startIndex = itemEnd + 7;
        }

        return items;
    }

    public void SaveData(List<string> data)
    {
        // Convert List to XML
        Console.WriteLine("  [Adapter] Converting List<string> to XML");
        string xml = "<data>";
        foreach (var item in data)
        {
            xml += $"<item>{item}</item>";
        }
        xml += "</data>";

        _xmlProvider.WriteXml(xml);
    }
}

/// <summary>
/// Modern implementation that already uses our interface
/// </summary>
public class JsonDataSource : IDataSource
{
    public List<string> GetData()
    {
        Console.WriteLine("  [Adapter] Reading data from JSON format");
        return new List<string> { "JSON Record 1", "JSON Record 2", "JSON Record 3" };
    }

    public void SaveData(List<string> data)
    {
        Console.WriteLine($"  [Adapter] Writing data to JSON: [{string.Join(", ", data)}]");
    }
}

#endregion

/// <summary>
/// Example demonstrating Adapter pattern
/// </summary>
public static class AdapterExample
{
    public static void Run()
    {
        Console.WriteLine();
        Console.WriteLine("5. ADAPTER PATTERN - Makes incompatible interfaces work together");
        Console.WriteLine("-".PadRight(70, '-'));
        Console.WriteLine();

        // Example 1: Payment Gateway Integration
        Console.WriteLine("Example 1: Payment Gateway Integration");
        Console.WriteLine();

        // Use modern payment processor
        IPaymentProcessor stripeProcessor = new StripePaymentProcessor();
        Console.WriteLine("  Processing with Stripe:");
        stripeProcessor.ProcessPayment(100.00m, "USD", "user@example.com");
        Console.WriteLine();

        // Use legacy payment system through adapter
        var legacySystem = new LegacyPaymentSystem();
        IPaymentProcessor legacyProcessor = new LegacyPaymentAdapter(legacySystem);
        Console.WriteLine("  Processing with Legacy System (via Adapter):");
        legacyProcessor.ProcessPayment(150.00m, "EUR", "customer@company.com");
        Console.WriteLine($"  Transaction ID: {legacyProcessor.GetTransactionId()}");
        Console.WriteLine();

        legacyProcessor.RefundPayment(legacyProcessor.GetTransactionId(), 50.00m);

        Console.WriteLine();

        // Example 2: Media Player Integration
        Console.WriteLine("Example 2: Universal Media Player");
        Console.WriteLine();

        // Create adapters for different player libraries
        IMediaPlayer videoPlayer = new VideoPlayerAdapter(new ThirdPartyVideoPlayer());
        IMediaPlayer audioPlayer = new AudioPlayerAdapter(new ThirdPartyAudioPlayer());

        // Use same interface for both
        Console.WriteLine("  Playing video:");
        videoPlayer.Play("movie.mp4");
        videoPlayer.SetVolume(75);
        videoPlayer.Stop();

        Console.WriteLine();

        Console.WriteLine("  Playing audio:");
        audioPlayer.Play("song.mp3");
        audioPlayer.SetVolume(60);
        audioPlayer.Stop();

        Console.WriteLine();

        // Example 3: Data Source Integration
        Console.WriteLine("Example 3: Unified Data Access");
        Console.WriteLine();

        // Modern JSON data source
        IDataSource jsonSource = new JsonDataSource();
        Console.WriteLine("  Using JSON data source:");
        var jsonData = jsonSource.GetData();
        Console.WriteLine($"  Retrieved {jsonData.Count} items");

        Console.WriteLine();

        // Legacy XML data source through adapter
        var xmlProvider = new XmlDataProvider();
        IDataSource xmlSource = new XmlDataAdapter(xmlProvider);
        Console.WriteLine("  Using XML data source (via Adapter):");
        var xmlData = xmlSource.GetData();
        Console.WriteLine($"  Retrieved {xmlData.Count} items:");
        foreach (var item in xmlData)
        {
            Console.WriteLine($"    - {item}");
        }

        Console.WriteLine();

        Console.WriteLine("  Saving new data to XML:");
        xmlSource.SaveData(new List<string> { "New Item 1", "New Item 2" });

        Console.WriteLine();
        Console.WriteLine("  Key Benefit: Use legacy code with modern interfaces!");
    }
}
