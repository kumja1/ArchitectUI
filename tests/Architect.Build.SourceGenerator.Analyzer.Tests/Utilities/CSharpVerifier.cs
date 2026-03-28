using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace Architect.Build.SourceGenerator.Analyzer.Tests.Utilities;

public abstract class CSharpVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic() =>
        CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, DefaultVerifier>.Diagnostic();

    public static DiagnosticResult Diagnostic(string diagnosticId) =>
        CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, DefaultVerifier>.Diagnostic(diagnosticId);

    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) =>
        new(descriptor);

    public static DiagnosticResult CompilerError(string errorIdentifier) =>
        new(errorIdentifier, DiagnosticSeverity.Error);

    public static Task VerifyAnalyzerAsyncV2(
        string source,
        params DiagnosticResult[] diagnostics) =>
        VerifyAnalyzerAsyncV2(new[] { source }, diagnostics);

    private static Task VerifyAnalyzerAsyncV2(
        string[] sources,
        params DiagnosticResult[] diagnostics)
    {
        TestV2 test = new();

        foreach (var source in sources)
            test.TestState.Sources.Add(source);

        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
        return test.RunAsync();
    }

    public static Task VerifyAnalyzerAsyncV2(
        (string filename, string content)[] sources,
        params DiagnosticResult[] diagnostics)
    {
        TestV2 test = new();
        test.TestState.Sources.AddRange(sources.Select(s => (s.filename, SourceText.From(s.content))));
        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
        return test.RunAsync();
    }

    public static Task VerifyCodeFixAsyncV2(
        string before,
        string after,
        int? codeActionIndex = null,
        params DiagnosticResult[] diagnostics)
    {
        TestV2 test = new()
        {
            TestCode = before,
            FixedCode = after,
            CodeActionIndex = codeActionIndex
        };
        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
        return test.RunAsync();
    }

    private class TestV2 : CSharpCodeFixTest<TAnalyzer, EmptyCodeFixProvider, DefaultVerifier>
    {
        public TestV2()
        {
            ReferenceAssemblies = CodeAnalyzerHelper.CurrentXunitV2;

            // xunit diagnostics are reported in both normal and generated code
            TestBehaviors |= TestBehaviors.SkipGeneratedCodeCheck;
        }

        protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
        {
            TAnalyzer analyzer = new();
            foreach (CodeFixProvider provider in CodeFixProviderDiscovery.GetCodeFixProviders(Language))
            {
                if (analyzer.SupportedDiagnostics.Any(descriptor =>
                        provider.FixableDiagnosticIds.Contains(descriptor.Id)))
                    yield return provider;
            }
        }
    }
}