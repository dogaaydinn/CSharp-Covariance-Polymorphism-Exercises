namespace AdvancedConcepts.SourceGenerators.Tests;

/// <summary>
/// Comprehensive tests for AutoMapGenerator source generator.
/// Tests mapping generation, reverse mapping, ignore attributes, and custom property names.
/// </summary>
public class AutoMapGeneratorTests
{
    [Fact]
    public async Task AutoMapGenerator_Should_Generate_Basic_Mapping_Method()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "public static TargetDto ToTargetDto",
            "Name = source.Name",
            "Age = source.Age"
        );
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Generate_Reverse_Mapping_By_Default()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string Name { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "public static TargetDto ToTargetDto",
            "public static Source ToSource"
        );
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Not_Generate_Reverse_Mapping_When_Disabled()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto), GenerateReverseMap = false)]
    public class Source
    {
        public string Name { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<AutoMapGenerator>(source);
        var generatedSource = string.Join("\n", GeneratorTestHelper.GetGeneratedSources(runResult));

        generatedSource.Should().Contain("public static TargetDto ToTargetDto");
        generatedSource.Should().NotContain("public static Source ToSource");
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Ignore_Properties_With_AutoMapIgnore_Attribute()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string Name { get; set; }

        [AutoMapIgnore]
        public string Password { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<AutoMapGenerator>(source);
        var generatedSource = string.Join("\n", GeneratorTestHelper.GetGeneratedSources(runResult));

        generatedSource.Should().Contain("Name = source.Name");
        generatedSource.Should().NotContain("Password = source.Password");
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Map_To_Custom_Property_Name()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        [AutoMapProperty(""FullName"")]
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class TargetDto
    {
        public string FullName { get; set; }
        public int Age { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "FullName = source.Name",
            "Age = source.Age"
        );
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Handle_Multiple_Properties()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }

    public class TargetDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "FirstName = source.FirstName",
            "LastName = source.LastName",
            "Age = source.Age",
            "Email = source.Email",
            "IsActive = source.IsActive"
        );
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Ignore_Missing_Properties_When_Enabled()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto), IgnoreMissingProperties = true)]
    public class Source
    {
        public string Name { get; set; }
        public string ExtraProperty { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<AutoMapGenerator>(source);
        GeneratorTestHelper.AssertNoDiagnostics(runResult);

        var generatedSource = string.Join("\n", GeneratorTestHelper.GetGeneratedSources(runResult));
        generatedSource.Should().Contain("Name = source.Name");
        generatedSource.Should().NotContain("ExtraProperty");
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Generate_Extension_Method_Class()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class UserModel
    {
        public string Name { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "public static class UserModelMappingExtensions",
            "public static TargetDto ToTargetDto(this TestNamespace.UserModel source)"
        );
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Handle_Null_Check()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string Name { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "if (source == null)",
            "throw new ArgumentNullException(nameof(source));"
        );
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Support_Multiple_Target_Types()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto1))]
    [AutoMap(typeof(TargetDto2))]
    public class Source
    {
        public string Name { get; set; }
    }

    public class TargetDto1
    {
        public string Name { get; set; }
    }

    public class TargetDto2
    {
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "public static TargetDto1 ToTargetDto1",
            "public static TargetDto2 ToTargetDto2"
        );
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Handle_Different_Types()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public long Id { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public long Id { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "Name = source.Name",
            "Age = source.Age",
            "Salary = source.Salary",
            "IsActive = source.IsActive",
            "Id = source.Id"
        );
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Only_Map_Public_Properties()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string PublicProperty { get; set; }
        private string PrivateProperty { get; set; }
        protected string ProtectedProperty { get; set; }
    }

    public class TargetDto
    {
        public string PublicProperty { get; set; }
        public string PrivateProperty { get; set; }
        public string ProtectedProperty { get; set; }
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<AutoMapGenerator>(source);
        var generatedSource = string.Join("\n", GeneratorTestHelper.GetGeneratedSources(runResult));

        generatedSource.Should().Contain("PublicProperty = source.PublicProperty");
        generatedSource.Should().NotContain("PrivateProperty");
        generatedSource.Should().NotContain("ProtectedProperty");
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Include_XML_Documentation()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string Name { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "/// <summary>",
            "/// Maps Source to TargetDto.",
            "/// </summary>"
        );
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Generate_Nullable_Enable_Directive()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string Name { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "#nullable enable"
        );
    }

    [Fact]
    public async Task AutoMapGenerator_Should_Generate_Auto_Generated_Comment()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string Name { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<AutoMapGenerator>(
            source,
            "// <auto-generated/>"
        );
    }

    [Fact]
    public void AutoMapGenerator_Should_Not_Produce_Diagnostics()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;

namespace TestNamespace
{
    [AutoMap(typeof(TargetDto))]
    public class Source
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TargetDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<AutoMapGenerator>(source);
        GeneratorTestHelper.AssertNoDiagnostics(runResult);
    }
}
