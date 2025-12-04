namespace AdvancedCsharpConcepts.Advanced.DesignPatterns;

/// <summary>
/// Builder Pattern - Separates construction from representation.
/// NVIDIA-style: Complex object creation with validation and fluent API.
/// </summary>
public static class BuilderPattern
{
    /// <summary>
    /// Complex product: Computer with many optional components.
    /// </summary>
    public class Computer
    {
        // Required properties
        public string CPU { get; init; } = string.Empty;
        public string Motherboard { get; init; } = string.Empty;
        public int RAM { get; init; } // in GB

        // Optional properties
        public string? GPU { get; init; }
        public int? Storage { get; init; } // in GB
        public bool HasSSD { get; init; }
        public bool HasWifi { get; init; }
        public string? CoolingSystem { get; init; }
        public int PowerSupply { get; init; } = 500; // Watts

        public string GetSpecifications()
        {
            var specs = $"CPU: {CPU}\n" +
                       $"Motherboard: {Motherboard}\n" +
                       $"RAM: {RAM}GB\n" +
                       $"Power Supply: {PowerSupply}W";

            if (GPU != null)
                specs += $"\nGPU: {GPU}";
            if (Storage.HasValue)
                specs += $"\nStorage: {Storage}GB {(HasSSD ? "SSD" : "HDD")}";
            if (HasWifi)
                specs += "\nWiFi: Enabled";
            if (CoolingSystem != null)
                specs += $"\nCooling: {CoolingSystem}";

            return specs;
        }
    }

    /// <summary>
    /// Traditional Builder - Mutable approach.
    /// </summary>
    public class ComputerBuilder
    {
        private string _cpu = string.Empty;
        private string _motherboard = string.Empty;
        private int _ram;
        private string? _gpu;
        private int? _storage;
        private bool _hasSSD;
        private bool _hasWifi;
        private string? _coolingSystem;
        private int _powerSupply = 500;

        public ComputerBuilder WithCPU(string cpu)
        {
            _cpu = cpu;
            return this;
        }

        public ComputerBuilder WithMotherboard(string motherboard)
        {
            _motherboard = motherboard;
            return this;
        }

        public ComputerBuilder WithRAM(int ram)
        {
            if (ram <= 0)
                throw new ArgumentException("RAM must be positive", nameof(ram));
            _ram = ram;
            return this;
        }

        public ComputerBuilder WithGPU(string gpu)
        {
            _gpu = gpu;
            return this;
        }

        public ComputerBuilder WithStorage(int storage, bool ssd = true)
        {
            if (storage <= 0)
                throw new ArgumentException("Storage must be positive", nameof(storage));
            _storage = storage;
            _hasSSD = ssd;
            return this;
        }

        public ComputerBuilder WithWifi()
        {
            _hasWifi = true;
            return this;
        }

        public ComputerBuilder WithCooling(string coolingSystem)
        {
            _coolingSystem = coolingSystem;
            return this;
        }

        public ComputerBuilder WithPowerSupply(int watts)
        {
            if (watts < 300)
                throw new ArgumentException("Power supply must be at least 300W", nameof(watts));
            _powerSupply = watts;
            return this;
        }

        public Computer Build()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(_cpu))
                throw new InvalidOperationException("CPU is required");
            if (string.IsNullOrWhiteSpace(_motherboard))
                throw new InvalidOperationException("Motherboard is required");
            if (_ram == 0)
                throw new InvalidOperationException("RAM is required");

            return new Computer
            {
                CPU = _cpu,
                Motherboard = _motherboard,
                RAM = _ram,
                GPU = _gpu,
                Storage = _storage,
                HasSSD = _hasSSD,
                HasWifi = _hasWifi,
                CoolingSystem = _coolingSystem,
                PowerSupply = _powerSupply
            };
        }
    }

    /// <summary>
    /// Modern C# Builder - Using init-only properties and records.
    /// </summary>
    public record ServerConfig
    {
        public required string ServerName { get; init; }
        public required int Port { get; init; }
        public string Host { get; init; } = "localhost";
        public bool UseSSL { get; init; }
        public int MaxConnections { get; init; } = 100;
        public int TimeoutSeconds { get; init; } = 30;
        public string? LogPath { get; init; }

        public static ServerConfigBuilder Builder => new();

        public class ServerConfigBuilder
        {
            private string? _serverName;
            private int _port;
            private string _host = "localhost";
            private bool _useSSL;
            private int _maxConnections = 100;
            private int _timeoutSeconds = 30;
            private string? _logPath;

            public ServerConfigBuilder WithServerName(string name)
            {
                _serverName = name;
                return this;
            }

            public ServerConfigBuilder WithPort(int port)
            {
                _port = port;
                return this;
            }

            public ServerConfigBuilder WithHost(string host)
            {
                _host = host;
                return this;
            }

            public ServerConfigBuilder WithSSL()
            {
                _useSSL = true;
                return this;
            }

            public ServerConfigBuilder WithMaxConnections(int max)
            {
                _maxConnections = max;
                return this;
            }

            public ServerConfigBuilder WithTimeout(int seconds)
            {
                _timeoutSeconds = seconds;
                return this;
            }

            public ServerConfigBuilder WithLogging(string path)
            {
                _logPath = path;
                return this;
            }

            public ServerConfig Build()
            {
                if (string.IsNullOrWhiteSpace(_serverName))
                    throw new InvalidOperationException("Server name is required");
                if (_port <= 0 || _port > 65535)
                    throw new InvalidOperationException("Invalid port number");

                return new ServerConfig
                {
                    ServerName = _serverName,
                    Port = _port,
                    Host = _host,
                    UseSSL = _useSSL,
                    MaxConnections = _maxConnections,
                    TimeoutSeconds = _timeoutSeconds,
                    LogPath = _logPath
                };
            }
        }
    }

    /// <summary>
    /// Demonstrates the Builder Pattern.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("=== Builder Pattern Examples ===\n");

        // Traditional Builder
        Console.WriteLine("1. Traditional Builder Pattern:");
        var gamingPC = new ComputerBuilder()
            .WithCPU("Intel Core i9-13900K")
            .WithMotherboard("ASUS ROG Maximus")
            .WithRAM(32)
            .WithGPU("NVIDIA RTX 4090")
            .WithStorage(2000, ssd: true)
            .WithWifi()
            .WithCooling("Liquid Cooling")
            .WithPowerSupply(1000)
            .Build();

        Console.WriteLine("Gaming PC Specifications:");
        Console.WriteLine(gamingPC.GetSpecifications());

        // Office PC
        Console.WriteLine("\n2. Office PC (Minimal Configuration):");
        var officePC = new ComputerBuilder()
            .WithCPU("Intel Core i5-12400")
            .WithMotherboard("MSI B660M")
            .WithRAM(16)
            .WithStorage(512, ssd: true)
            .Build();

        Console.WriteLine(officePC.GetSpecifications());

        // Modern Builder with Records
        Console.WriteLine("\n3. Modern Builder with Records:");
        var webServer = ServerConfig.Builder
            .WithServerName("WebAPI-Production")
            .WithPort(8080)
            .WithHost("api.example.com")
            .WithSSL()
            .WithMaxConnections(500)
            .WithTimeout(60)
            .WithLogging("/var/log/webapi.log")
            .Build();

        Console.WriteLine($"Server: {webServer.ServerName}");
        Console.WriteLine($"Endpoint: {(webServer.UseSSL ? "https" : "http")}://{webServer.Host}:{webServer.Port}");
        Console.WriteLine($"Max Connections: {webServer.MaxConnections}");
        Console.WriteLine($"Timeout: {webServer.TimeoutSeconds}s");
        Console.WriteLine($"Logging: {webServer.LogPath ?? "Disabled"}");
    }
}
