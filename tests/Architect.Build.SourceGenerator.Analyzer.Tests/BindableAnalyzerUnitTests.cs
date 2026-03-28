using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verify = Architect.Build.SourceGenerator.Analyzer.Tests.Utilities.CSharpVerifier<Architect.Build.SourceGenerator.Analyzer.BindableAnalyzer>;

namespace Architect.Build.SourceGenerator.Analyzer.Tests;

public class BindableAnalyzerUnitTest
{
    [Fact]
    public async Task NoDiagnostics_ForCorrectImplementation()
    {
        const string test = """
            using Architect.Build.SourceGenerator;

            namespace Architect.Build.SourceGenerator
            {
                public class BindableAttribute : System.Attribute {}
                public class BindablePropertyAttribute : System.Attribute {}
                public class BindableObject {}
            }

            [Bindable]
            class MyClass : BindableObject
            {
                [BindableProperty]
                private int _myField;

                [BindableProperty]
                private partial int MyProperty { get; set; }
            }
            """;
        await Verify.VerifyAnalyzerAsyncV2(test);
    }

    [Fact]
    public async Task ClassMustInheritBindableObject_TriggersDiagnosticAndFix()
    {
        const string test = """
            using Architect.Build.SourceGenerator;

            namespace Architect.Build.SourceGenerator
            {
                public class BindableAttribute : System.Attribute {}
                public class BindableObject {}
            }

            [Bindable]
            class [|MyClass|]
            {
            }
            """;

        const string fixtest = """
            using Architect.Build.SourceGenerator;

            namespace Architect.Build.SourceGenerator
            {
                public class BindableAttribute : System.Attribute {}
                public class BindableObject {}
            }

            [Bindable]
            class MyClass : BindableObject
            {
            }
            """;

        await Verify.VerifyCodeFixAsyncV2(test, fixtest);
    }

    [Fact]
    public async Task MemberShouldBePrivate_TriggersDiagnostic()
    {
        const string test = """
            using Architect.Build.SourceGenerator;

            namespace Architect.Build.SourceGenerator
            {
                public class BindableAttribute : System.Attribute {}
                public class BindablePropertyAttribute : System.Attribute {}
                public class BindableObject {}
            }

            [Bindable]
            class MyClass : BindableObject
            {
                [BindableProperty]
                public int [|_myField|];
            }
            """;

        DiagnosticResult expected = Verify.Diagnostic("AG0006").WithArguments("public int _myField;");
        await Verify.VerifyAnalyzerAsyncV2(test, expected);
    }

    [Fact]
    public async Task MemberNameIsIdentical_TriggersDiagnosticAndFix()
    {
        const string test = """
            using Architect.Build.SourceGenerator;

            namespace Architect.Build.SourceGenerator
            {
                public class BindableAttribute : System.Attribute {}
                public class BindablePropertyAttribute : System.Attribute {}
                public class BindableObject {}
            }

            [Bindable]
            class MyClass : BindableObject
            {
                [BindableProperty]
                private partial int [|Myproperty|] { get; set; }
            }
            """;

        const string fixtest = """
            using Architect.Build.SourceGenerator;

            namespace Architect.Build.SourceGenerator
            {
                public class BindableAttribute : System.Attribute {}
                public class BindablePropertyAttribute : System.Attribute {}
                public class BindableObject {}
            }

            [Bindable]
            class MyClass : BindableObject
            {
                [BindableProperty]
                private partial int _myproperty { get; set; }
            }
            """;

        await Verify.VerifyCodeFixAsyncV2(test, fixtest);
    }
    
    [Fact]
    public async Task MemberCanBeField_TriggersDiagnosticAndFix()
    {
        const string test = """
            using Architect.Build.SourceGenerator;

            namespace Architect.Build.SourceGenerator
            {
                public class BindableAttribute : System.Attribute {}
                public class BindablePropertyAttribute : System.Attribute {}
                public class BindableObject {}
            }

            [Bindable]
            class MyClass : BindableObject
            {
                [BindableProperty]
                private int [|MyProperty|] { get; set; }
            }
            """;

        const string fixtest = """
            using Architect.Build.SourceGenerator;

            namespace Architect.Build.SourceGenerator
            {
                public class BindableAttribute : System.Attribute {}
                public class BindablePropertyAttribute : System.Attribute {}
                public class BindableObject {}
            }

            [Bindable]
            class MyClass : BindableObject
            {
                [BindableProperty]
                private int MyProperty;
            }
            """;

        await Verify.VerifyCodeFixAsyncV2(test, fixtest);
    }
}