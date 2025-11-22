namespace AdvancedCsharpConcepts.Advanced.DesignPatterns;

/// <summary>
/// Adapter Pattern - Converts one interface to another to enable compatibility.
/// Silicon Valley best practice: Integrate legacy systems with modern code.
/// </summary>
public static class AdapterPattern
{
    // Target Interface (what our modern code expects)
    public interface IMediaPlayer
    {
        void Play(string audioType, string fileName);
        void Stop();
        string GetCurrentStatus();
    }

    // Legacy System (incompatible interface)
    public interface ILegacyAudioPlayer
    {
        void PlayMp3(string fileName);
        void StopMp3();
    }

    public interface IAdvancedMediaPlayer
    {
        void PlayVlc(string fileName);
        void PlayMp4(string fileName);
        void StopPlayback();
    }

    // Adaptee 1: Legacy MP3 Player
    public class LegacyMp3Player : ILegacyAudioPlayer
    {
        public void PlayMp3(string fileName)
        {
            Console.WriteLine($"[LegacyMp3Player] Playing MP3 file: {fileName}");
        }

        public void StopMp3()
        {
            Console.WriteLine("[LegacyMp3Player] Stopped MP3 playback");
        }
    }

    // Adaptee 2: Advanced Media Player
    public class AdvancedMediaPlayer : IAdvancedMediaPlayer
    {
        public void PlayVlc(string fileName)
        {
            Console.WriteLine($"[AdvancedMediaPlayer] Playing VLC file: {fileName}");
        }

        public void PlayMp4(string fileName)
        {
            Console.WriteLine($"[AdvancedMediaPlayer] Playing MP4 file: {fileName}");
        }

        public void StopPlayback()
        {
            Console.WriteLine("[AdvancedMediaPlayer] Stopped playback");
        }
    }

    // Adapter: Makes legacy players compatible with modern interface
    public class MediaAdapter : IMediaPlayer
    {
        private readonly ILegacyAudioPlayer? _legacyPlayer;
        private readonly IAdvancedMediaPlayer? _advancedPlayer;
        private readonly string _audioType;
        private bool _isPlaying;

        public MediaAdapter(string audioType)
        {
            _audioType = audioType.ToLower();

            if (_audioType == "mp3")
            {
                _legacyPlayer = new LegacyMp3Player();
            }
            else if (_audioType == "vlc" || _audioType == "mp4")
            {
                _advancedPlayer = new AdvancedMediaPlayer();
            }
            else
            {
                throw new ArgumentException($"Unsupported audio type: {audioType}");
            }
        }

        public void Play(string audioType, string fileName)
        {
            var type = audioType.ToLower();

            if (type == "mp3" && _legacyPlayer != null)
            {
                _legacyPlayer.PlayMp3(fileName);
                _isPlaying = true;
            }
            else if (type == "vlc" && _advancedPlayer != null)
            {
                _advancedPlayer.PlayVlc(fileName);
                _isPlaying = true;
            }
            else if (type == "mp4" && _advancedPlayer != null)
            {
                _advancedPlayer.PlayMp4(fileName);
                _isPlaying = true;
            }
            else
            {
                throw new InvalidOperationException($"Cannot play {type} with current adapter");
            }
        }

        public void Stop()
        {
            if (_legacyPlayer != null)
            {
                _legacyPlayer.StopMp3();
            }
            else if (_advancedPlayer != null)
            {
                _advancedPlayer.StopPlayback();
            }
            _isPlaying = false;
        }

        public string GetCurrentStatus()
        {
            return _isPlaying ? $"Playing {_audioType}" : "Stopped";
        }
    }

    // Modern Audio Player (uses adapters internally)
    public class AudioPlayer : IMediaPlayer
    {
        private IMediaPlayer? _adapter;
        private bool _isPlaying;

        public void Play(string audioType, string fileName)
        {
            var type = audioType.ToLower();

            // Play MP3 directly (native support)
            if (type == "mp3")
            {
                Console.WriteLine($"[AudioPlayer] Playing MP3 file: {fileName}");
                _isPlaying = true;
            }
            // Use adapter for other formats
            else if (type == "vlc" || type == "mp4")
            {
                _adapter = new MediaAdapter(type);
                _adapter.Play(type, fileName);
                _isPlaying = true;
            }
            else
            {
                Console.WriteLine($"[AudioPlayer] ERROR: Invalid media type '{type}'");
            }
        }

        public void Stop()
        {
            if (_adapter != null)
            {
                _adapter.Stop();
                _adapter = null;
            }
            else
            {
                Console.WriteLine("[AudioPlayer] Stopped playback");
            }
            _isPlaying = false;
        }

        public string GetCurrentStatus()
        {
            if (_adapter != null)
            {
                return _adapter.GetCurrentStatus();
            }
            return _isPlaying ? "Playing" : "Stopped";
        }
    }

    // Real-world example: Database Adapter
    public interface IDatabase
    {
        Task<IEnumerable<string>> QueryAsync(string sql, CancellationToken cancellationToken = default);
        Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken = default);
    }

    // Legacy MongoDB client (different interface)
    public class MongoDbClient
    {
        public async Task<List<string>> Find(string collection, string filter, CancellationToken cancellationToken = default)
        {
            await Task.Delay(10, cancellationToken);
            return new List<string> { "Document1", "Document2" };
        }

        public async Task<int> Insert(string collection, string document, CancellationToken cancellationToken = default)
        {
            await Task.Delay(10, cancellationToken);
            return 1;
        }
    }

    // Adapter: Makes MongoDB compatible with SQL interface
    public class MongoDbAdapter : IDatabase
    {
        private readonly MongoDbClient _mongoClient;

        public MongoDbAdapter(MongoDbClient mongoClient)
        {
            _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
        }

        public async Task<IEnumerable<string>> QueryAsync(string sql, CancellationToken cancellationToken = default)
        {
            // Parse SQL to MongoDB query (simplified)
            var collection = "users"; // Extracted from SQL
            var filter = "{}"; // Converted from WHERE clause

            var results = await _mongoClient.Find(collection, filter, cancellationToken);
            return results;
        }

        public async Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken = default)
        {
            // Parse SQL to MongoDB command (simplified)
            var collection = "users";
            var document = "{}";

            return await _mongoClient.Insert(collection, document, cancellationToken);
        }
    }

    /// <summary>
    /// Demonstrates the Adapter Pattern.
    /// </summary>
    public static async Task RunExample(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Adapter Pattern Examples ===\n");

        // Example 1: Media Player Adapter
        Console.WriteLine("1. Media Player Adapter:");
        var audioPlayer = new AudioPlayer();

        audioPlayer.Play("mp3", "song.mp3");
        Console.WriteLine($"  Status: {audioPlayer.GetCurrentStatus()}");
        audioPlayer.Stop();

        audioPlayer.Play("mp4", "video.mp4");
        Console.WriteLine($"  Status: {audioPlayer.GetCurrentStatus()}");
        audioPlayer.Stop();

        audioPlayer.Play("vlc", "movie.vlc");
        Console.WriteLine($"  Status: {audioPlayer.GetCurrentStatus()}");
        audioPlayer.Stop();

        // Example 2: Unsupported format
        Console.WriteLine("\n2. Unsupported Format:");
        audioPlayer.Play("avi", "video.avi");

        // Example 3: Database Adapter
        Console.WriteLine("\n3. Database Adapter (MongoDB to SQL interface):");
        var mongoClient = new MongoDbClient();
        IDatabase database = new MongoDbAdapter(mongoClient);

        var results = await database.QueryAsync("SELECT * FROM users", cancellationToken);
        Console.WriteLine($"  Query results: {string.Join(", ", results)}");

        var affected = await database.ExecuteAsync("INSERT INTO users VALUES (...)", cancellationToken);
        Console.WriteLine($"  Rows affected: {affected}");

        // Example 4: Multiple adapters
        Console.WriteLine("\n4. Multiple Adapters in Action:");
        var formats = new[] { "mp3", "vlc", "mp4", "mp3" };
        var player = new AudioPlayer();

        foreach (var format in formats)
        {
            player.Play(format, $"file.{format}");
            await Task.Delay(100, cancellationToken);
            player.Stop();
        }
    }
}
