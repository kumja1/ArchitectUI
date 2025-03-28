using System.Collections.Immutable;
using Architect.Build.SourceGenerator.Analyzer.CodeFixes;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.Composition;

namespace Architect.Build.SourceGenerator.Analyzer.Test.Utilities
{
    static class CodeFixProviderDiscovery
    {
        static readonly Lazy<IExportProviderFactory> ExportProviderFactory;

        static CodeFixProviderDiscovery()
        {
            ExportProviderFactory = new Lazy<IExportProviderFactory>(
                () =>
                {
                    var discovery = new AttributedPartDiscovery(
                        Resolver.DefaultInstance,
                        isNonPublicSupported: true
                    );
                    var parts = Task.Run(
                            () =>
                                discovery.CreatePartsAsync(typeof(BindableCodeFixProvider).Assembly)
                        )
                        .GetAwaiter()
                        .GetResult();
                    var catalog = ComposableCatalog
                        .Create(Resolver.DefaultInstance)
                        .AddParts(parts);

                    var configuration = CompositionConfiguration.Create(catalog);
                    var runtimeComposition = RuntimeComposition.CreateRuntimeComposition(
                        configuration
                    );
                    return runtimeComposition.CreateExportProviderFactory();
                },
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        public static IEnumerable<CodeFixProvider> GetCodeFixProviders(string language)
        {
            var exportProvider = ExportProviderFactory.Value.CreateExportProvider();
            var exports = exportProvider.GetExports<CodeFixProvider, LanguageMetadata>();

            return exports
                .Where(export => export.Metadata.Languages.Contains(language))
                .Select(export => export.Value);
        }

        class LanguageMetadata
        {
            public LanguageMetadata(IDictionary<string, object> data)
            {
                if (
                    !data.TryGetValue(
                        nameof(ExportCodeFixProviderAttribute.Languages),
                        out var languages
                    )
                )
                    languages = Array.Empty<string>();

                Languages = [.. (string[])languages];
            }

            public ImmutableArray<string> Languages { get; }
        }
    }
}
