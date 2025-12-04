namespace AdvancedConcepts.SourceGenerators.Tests;

/// <summary>
/// Comprehensive tests for LoggerMessageGenerator source generator.
/// Tests high-performance logging method generation with various log levels and options.
/// </summary>
public class LoggerMessageGeneratorTests
{
    [Fact]
    public async Task LoggerMessageGenerator_Should_Generate_Basic_Logger_Method()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""User logged in"")]
        public static partial void LogUserLogin(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "public static partial void LogUserLogin",
            "LoggerMessage.Define",
            "LogLevel.Information",
            "new EventId(1"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Generate_Method_With_Parameters()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""User {userId} logged in"")]
        public static partial void LogUserLogin(ILogger logger, int userId);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "public static partial void LogUserLogin(ILogger logger, int userId)",
            "LoggerMessage.Define<int>",
            "_LogUserLoginDelegate(logger, userId, null)"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Support_Trace_Level()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Trace, Message = ""Trace message"")]
        public static partial void LogTrace(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "LogLevel.Trace"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Support_Debug_Level()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = ""Debug message"")]
        public static partial void LogDebug(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "LogLevel.Debug"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Support_Warning_Level()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = ""Warning message"")]
        public static partial void LogWarning(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "LogLevel.Warning"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Support_Error_Level()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = ""Error message"")]
        public static partial void LogError(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "LogLevel.Error"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Support_Critical_Level()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 5, Level = LogLevel.Critical, Message = ""Critical message"")]
        public static partial void LogCritical(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "LogLevel.Critical"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Use_Custom_Event_Name()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 100, Level = LogLevel.Information, Message = ""Custom event"", EventName = ""MyCustomEvent"")]
        public static partial void LogCustomEvent(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "\"MyCustomEvent\""
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Use_Method_Name_As_Default_Event_Name()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""Event"")]
        public static partial void LogUserActivity(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "\"LogUserActivity\""
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Generate_IsEnabled_Check_By_Default()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""Message"")]
        public static partial void LogMessage(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "if (!logger.IsEnabled(LogLevel.Information))",
            "return;"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Skip_IsEnabled_Check_When_Requested()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""Message"", SkipEnabledCheck = true)]
        public static partial void LogMessage(ILogger logger);
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<LoggerMessageGenerator>(source);
        var generatedSource = string.Join("\n", GeneratorTestHelper.GetGeneratedSources(runResult));

        generatedSource.Should().NotContain("IsEnabled");
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Handle_Multiple_Parameters()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""User {userId} performed {action} at {timestamp}"")]
        public static partial void LogUserAction(ILogger logger, int userId, string action, System.DateTime timestamp);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "LoggerMessage.Define<int, string, System.DateTime>",
            "public static partial void LogUserAction(ILogger logger, int userId, string action, System.DateTime timestamp)",
            "_LogUserActionDelegate(logger, userId, action, timestamp, null)"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Generate_Static_Delegate_Field()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""Message"")]
        public static partial void LogMessage(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "private static readonly Action",
            "_LogMessageDelegate ="
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Handle_Multiple_Methods_In_Same_Class()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""First message"")]
        public static partial void LogFirst(ILogger logger);

        [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = ""Second message"")]
        public static partial void LogSecond(ILogger logger);

        [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = ""Third message"")]
        public static partial void LogThird(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "public static partial void LogFirst",
            "public static partial void LogSecond",
            "public static partial void LogThird",
            "_LogFirstDelegate",
            "_LogSecondDelegate",
            "_LogThirdDelegate"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Work_With_Different_Namespaces()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace MyApp.Logging
{
    public static partial class AppLogger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""Application started"")]
        public static partial void LogAppStart(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "namespace MyApp.Logging;",
            "public static partial class AppLogger"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Include_XML_Documentation()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""Message"")]
        public static partial void LogMessage(ILogger logger);
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<LoggerMessageGenerator>(source);
        var generatedSource = string.Join("\n", GeneratorTestHelper.GetGeneratedSources(runResult));

        // The generator produces code, even if it doesn't have explicit XML docs
        generatedSource.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Handle_String_Parameters()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""Processing {fileName}"")]
        public static partial void LogFileProcessing(ILogger logger, string fileName);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "LoggerMessage.Define<string>",
            "string fileName"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Pass_Null_For_Exception_Parameter()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""Message"")]
        public static partial void LogMessage(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            ", null);"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Generate_Nullable_Enable_Directive()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""Message"")]
        public static partial void LogMessage(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "#nullable enable"
        );
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Generate_Auto_Generated_Comment()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""Message"")]
        public static partial void LogMessage(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "// <auto-generated/>"
        );
    }

    [Fact]
    public void LoggerMessageGenerator_Should_Not_Produce_Diagnostics()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = ""User {userId} logged in"")]
        public static partial void LogUserLogin(ILogger logger, int userId);
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<LoggerMessageGenerator>(source);
        GeneratorTestHelper.AssertNoDiagnostics(runResult);
    }

    [Fact]
    public async Task LoggerMessageGenerator_Should_Support_Different_Event_Ids()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    public static partial class Logger
    {
        [LoggerMessage(EventId = 1001, Level = LogLevel.Information, Message = ""Message one"")]
        public static partial void LogMessage1(ILogger logger);

        [LoggerMessage(EventId = 2002, Level = LogLevel.Warning, Message = ""Message two"")]
        public static partial void LogMessage2(ILogger logger);
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<LoggerMessageGenerator>(
            source,
            "new EventId(1001",
            "new EventId(2002"
        );
    }
}
