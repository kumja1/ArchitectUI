using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Text;

namespace Architect.Build.SourceGenerator.Analyzer.Test.Utilities
{
    public class CSharpVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public static DiagnosticResult Diagnostic() =>
            CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, XUnitVerifier>.Diagnostic();

        public static DiagnosticResult Diagnostic(string diagnosticId) =>
            CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, XUnitVerifier>.Diagnostic(
                diagnosticId
            );

        public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) =>
            new(descriptor);

        public static DiagnosticResult CompilerError(string errorIdentifier) =>
            new(errorIdentifier, DiagnosticSeverity.Error);

        public static Task VerifyAnalyzerAsyncV2(
            string source,
            params DiagnosticResult[] diagnostics
        ) => VerifyAnalyzerAsyncV2([source], [], string.Empty, diagnostics);

        public static Task VerifyAnalyzerAsyncV2(
            string[] sources,
            (string filename, string content)[] generatedSources,
            params DiagnosticResult[] diagnostics
        ) => VerifyAnalyzerAsyncV2(sources, generatedSources, string.Empty, diagnostics);

        public static Task VerifyAnalyzerAsyncV2(
            string[] sources,
            (string filename, string content)[] generatedSources,
            string targetFramework,
            params DiagnosticResult[] diagnostics
        )
        {
            if (string.IsNullOrEmpty(targetFramework))
                targetFramework = "netstandard2.0";

            var test = new TestV2 { TargetFramework = targetFramework };

            foreach (var source in sources)
                test.TestState.Sources.Add(source);

            foreach (var generatedSource in generatedSources)
               test.TestState.Sources.Add(generatedSource);
            test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
            return test.RunAsync();
        }

        public static Task VerifyAnalyzerAsyncV2(
            (string filename, string content)[] sources,
            params DiagnosticResult[] diagnostics
        )
        {
            var test = new TestV2();
            test.TestState.Sources.AddRange(
                sources.Select(s => (s.filename, SourceText.From(s.content)))
            );
            test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
            return test.RunAsync();
        }

        public static Task VerifyCodeFixAsyncV2(
            string before,
            string after,
            int? codeActionIndex = null,
            params DiagnosticResult[] diagnostics
        )
        {
            var test = new TestV2
            {
                TestCode = before,
                FixedCode = after,
                CodeActionIndex = codeActionIndex,
            };
            test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
            return test.RunAsync();
        }

        public class TestV2 : CSharpCodeFixTest<TAnalyzer, EmptyCodeFixProvider, XUnitVerifier>
        {
            public string TargetFramework { get; set; } = "netstandard2.0";

            public TestV2()
            {
                ReferenceAssemblies = CodeAnalyzerHelper.CurrentXunitV2;
                MarkupOptions = MarkupOptions.UseFirstDescriptor;

                // xunit diagnostics are reported in both normal and generated code
                TestBehaviors |= TestBehaviors.SkipGeneratedCodeCheck;
            }

            protected override ParseOptions CreateParseOptions()
            {
                var parseOptions = base.CreateParseOptions();
                return parseOptions.WithFeatures(
                    parseOptions.Features.Concat(
                        [new KeyValuePair<string, string>("TargetFramework", TargetFramework)]
                    )
                );
            }

            protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
            {
                var analyzer = new TAnalyzer();

                foreach (var provider in CodeFixProviderDiscovery.GetCodeFixProviders(Language))
                    if (
                        analyzer.SupportedDiagnostics.Any(diagnostic =>
                            provider.FixableDiagnosticIds.Contains(diagnostic.Id)
                        )
                    )
                        yield return provider;
            }
        }
    }
}
