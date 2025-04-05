using Xunit;
using Verify = Architect.Build.SourceGenerator.Analyzer.Test.Utilities.CSharpVerifier<Architect.Build.SourceGenerator.Analyzer.BindableAnalyzer>;

namespace Architect.Build.SourceGenerator.Analyzer.Test;

public class AnalyzerUnitTestsTest
{
    private static readonly (string, string) GeneratorOutput = (
        "BindableAttributes.g.cs",
        """
    namespace Roslyn.Generated;
    
        [System.AttributeUsage(System.AttributeTargets.Class)]
        public class BindableObjectAttribute : System.Attribute { }

        [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
        public class BindablePropertyAttribute : System.Attribute { }
    
"""
    );

    [Fact]
    public async Task ClassMustBePartial_DiagnosticTriggered()
    {
        var test =
            @"
    namespace TestNamespace
    {
        using Roslyn.Generated;
        
        [BindableObject]
        public class [|TestClass|]
        {
        }
    }";

        var expected = Verify
            .Diagnostic(DiagnosticMessages.ClassMustBePartial)
            .WithLocation(6, 9)
            .WithArguments("TestClass");

        await Verify.VerifyAnalyzerAsyncV2([test], [GeneratorOutput], string.Empty, expected);
    }

    [Fact]
    public async Task ClassMustImplementIBindable_DiagnosticTriggered()
    {
        var test =
            @"
    namespace TestNamespace
    {
        using Roslyn.Generated;

        [BindableObject]
        public partial class [|TestClass|]
        {
        }
    }";

        var expected = Verify
            .Diagnostic(DiagnosticMessages.ClassMustImplementIBindable)
            .WithSpan(5, 29, 5, 38)
            .WithArguments("TestClass");

        await Verify.VerifyAnalyzerAsyncV2([test], [GeneratorOutput], string.Empty, expected);
    }

    [Fact]
    public async Task MemberMustBePartial_DiagnosticTriggered()
    {
        var test =
            @"
    namespace TestNamespace
    {
        using Roslyn.Generated;

        [BindableObject]
        public partial class TestClass
        {
          private string [|testField|];
        }
    }";

        var expected = Verify
            .Diagnostic(DiagnosticMessages.MemberMustBePartial)
            .WithSpan(7, 25, 7, 35)
            .WithArguments("testField");

        await Verify.VerifyAnalyzerAsyncV2([test], [GeneratorOutput], string.Empty, expected);
    }

    [Fact]
    public async Task MemberNameIsIdentical_DiagnosticTriggered()
    {
        var test =
            @"
    namespace TestNamespace
    {
        using Roslyn.Generated;

        [BindableObject]
        public partial class TestClass
        {
            [BindableProperty]
            public string [|PropertyName|] { get; set; }
        }
    }";

        var expected = Verify
            .Diagnostic(DiagnosticMessages.MemberNameIsIdentical)
            .WithSpan(7, 27, 7, 39)
            .WithArguments("PropertyName");

        await Verify.VerifyAnalyzerAsyncV2([test], [GeneratorOutput], string.Empty, expected);
    }

    [Fact]
    public async Task MemberCanBeField_DiagnosticTriggered()
    {
        var test =
            @"
    namespace TestNamespace
    {
        using Roslyn.Generated;

        [BindableObject]
        public partial class TestClass
        {
            [BindableProperty]
            public string [|FieldName|];
        }
    }";

        var expected = Verify
            .Diagnostic(DiagnosticMessages.MemberCanBeField)
            .WithSpan(7, 27, 7, 36)
            .WithArguments("FieldName");

        await Verify.VerifyAnalyzerAsyncV2([test], [GeneratorOutput], string.Empty, expected);
    }

    [Fact]
    public async Task MemberShouldBePrivate_DiagnosticTriggered()
    {
        var test =
            @"
    namespace TestNamespace
    {
        using Roslyn.Generated;

        [BindableObject]
        public partial class TestClass
        {
            [BindableProperty]
            public string [|FieldName|];
        }
    }";

        var expected = Verify
            .Diagnostic(DiagnosticMessages.MemberShouldBePrivate)
            .WithSpan(7, 27, 7, 36)
            .WithArguments("FieldName");

        await Verify.VerifyAnalyzerAsyncV2([test], [GeneratorOutput], string.Empty, expected);
    }
}
