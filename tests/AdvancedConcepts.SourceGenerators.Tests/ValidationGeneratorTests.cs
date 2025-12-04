namespace AdvancedConcepts.SourceGenerators.Tests;

/// <summary>
/// Comprehensive tests for ValidationGenerator source generator.
/// Tests validation method generation for Required, StringLength, EmailAddress, Range, and RegularExpression attributes.
/// </summary>
public class ValidationGeneratorTests
{
    [Fact]
    public async Task ValidationGenerator_Should_Generate_ValidationResult_Class()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "public class ValidationResult",
            "public bool IsValid { get; set; }",
            "public List<string> Errors { get; set; }"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Generate_Validate_Method()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "public ValidationResult Validate()",
            "var result = new ValidationResult { IsValid = true };"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Generate_IsValid_Method()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "public bool IsValid()",
            "return Validate().IsValid;"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Validate_Required_String_Property()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "if (string.IsNullOrWhiteSpace(Name))",
            "result.IsValid = false;",
            "result.Errors.Add(\"Name is required\");"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Validate_StringLength_Property()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [StringLength(100, MinimumLength = 5)]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "if (Name != null)",
            "if (Name.Length < 5 || Name.Length > 100)"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Validate_EmailAddress_Property()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "if (!string.IsNullOrEmpty(Email))",
            "var emailRegex = new Regex(",
            "if (!emailRegex.IsMatch(Email))"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Validate_Range_Property()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Range(18, 120)]
        public int Age { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "if (Age < 18 || Age > 120)",
            "result.Errors.Add(\"Age must be between 18 and 120\");"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Validate_RegularExpression_Property()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [RegularExpression(@""^\d{3}-\d{3}-\d{4}$"")]
        public string PhoneNumber { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "if (!string.IsNullOrEmpty(PhoneNumber))",
            "var regex = new Regex(",
            "if (!regex.IsMatch(PhoneNumber))"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Handle_Multiple_Validations_On_Same_Property()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "if (string.IsNullOrWhiteSpace(Name))",
            "if (Name != null)",
            "if (Name.Length < 3 || Name.Length > 100)"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Handle_Multiple_Properties()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Range(0, 150)]
        public int Age { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "if (string.IsNullOrWhiteSpace(Name))",
            "emailRegex",
            "if (Age < 0 || Age > 150)"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Use_Custom_Error_Message_For_Required()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required(ErrorMessage = ""Username is mandatory"")]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "result.Errors.Add(\"Username is mandatory\");"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Use_Custom_Error_Message_For_StringLength()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = ""Name must be 2-50 characters"")]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "result.Errors.Add(\"Name must be 2-50 characters\");"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Use_Custom_Error_Message_For_EmailAddress()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [EmailAddress(ErrorMessage = ""Please provide a valid email"")]
        public string Email { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "result.Errors.Add(\"Please provide a valid email\");"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Use_Custom_Error_Message_For_Range()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Range(1, 100, ErrorMessage = ""Value must be between 1 and 100"")]
        public int Value { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "result.Errors.Add(\"Value must be between 1 and 100\");"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Use_Custom_Error_Message_For_RegularExpression()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [RegularExpression(@""^\d{5}$"", ErrorMessage = ""Invalid ZIP code"")]
        public string ZipCode { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "result.Errors.Add(\"Invalid ZIP code\");"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Generate_Partial_Class()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "public partial class User"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Handle_Different_Namespaces()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    [Validate]
    public partial class Product
    {
        [Required]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "namespace MyApp.Models;",
            "public partial class Product"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Include_Required_Usings()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "using System;",
            "using System.Collections.Generic;",
            "using System.Text.RegularExpressions;"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Handle_StringLength_Without_MinimumLength()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [StringLength(50)]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "if (Name.Length < 0 || Name.Length > 50)"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Only_Validate_Public_Properties()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        public string PublicName { get; set; }

        [Required]
        private string PrivateName { get; set; }
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<ValidationGenerator>(source);
        var generatedSource = string.Join("\n", GeneratorTestHelper.GetGeneratedSources(runResult));

        generatedSource.Should().Contain("PublicName");
        generatedSource.Should().NotContain("PrivateName");
    }

    [Fact]
    public async Task ValidationGenerator_Should_Generate_Nullable_Enable_Directive()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "#nullable enable"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Generate_Auto_Generated_Comment()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        public string Name { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "// <auto-generated/>"
        );
    }

    [Fact]
    public void ValidationGenerator_Should_Not_Produce_Diagnostics()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Range(0, 150)]
        public int Age { get; set; }
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<ValidationGenerator>(source);
        GeneratorTestHelper.AssertNoDiagnostics(runResult);
    }

    [Fact]
    public async Task ValidationGenerator_Should_Handle_Decimal_Range()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class Product
    {
        [Range(0.01, 9999.99)]
        public decimal Price { get; set; }
    }
}";

        await GeneratorTestHelper.VerifyGeneratorContainsAsync<ValidationGenerator>(
            source,
            "if (Price < 0.01 || Price > 9999.99)"
        );
    }

    [Fact]
    public async Task ValidationGenerator_Should_Handle_Complex_Regex_Pattern()
    {
        var source = @"
using AdvancedConcepts.SourceGenerators;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    [Validate]
    public partial class User
    {
        [RegularExpression(@""^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$"")]
        public string Password { get; set; }
    }
}";

        var runResult = GeneratorTestHelper.RunGenerator<ValidationGenerator>(source);
        var generatedSource = string.Join("\n", GeneratorTestHelper.GetGeneratedSources(runResult));

        generatedSource.Should().Contain("var regex = new Regex(");
        generatedSource.Should().Contain("if (!regex.IsMatch(Password))");
    }
}
