using System.Collections.Immutable;
using Architect.Build.SourceGenerator.Analyzer.CodeFixes;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.Composition;

namespace Architect.Build.SourceGenerator.Analyzer.Tests.Utilities;

static class CodeFixProviderDiscovery
{
    private static readonly Lazy<IExportProviderFactory> _exportProviderFactory;

    static CodeFixProviderDiscovery()
    {
        _exportProviderFactory = new Lazy<IExportProviderFactory>(
            () =>
            {
                AttributedPartDiscovery discovery = new(Resolver.DefaultInstance, isNonPublicSupported: true);
                DiscoveredParts? parts = Task.Run(() => discovery.CreatePartsAsync(typeof(BindableCodeFixProvider).Assembly)).GetAwaiter().GetResult();
                ComposableCatalog? catalog = ComposableCatalog.Create(Resolver.DefaultInstance).AddParts(parts);

                CompositionConfiguration? configuration = CompositionConfiguration.Create(catalog);
                RuntimeComposition? runtimeComposition = RuntimeComposition.CreateRuntimeComposition(configuration);
                return runtimeComposition.CreateExportProviderFactory();
            },
            LazyThreadSafetyMode.ExecutionAndPublication
        );
    }

    public static IEnumerable<CodeFixProvider> GetCodeFixProviders(string language)
    {
        ExportProvider? exportProvider = _exportProviderFactory.Value.CreateExportProvider();
        IEnumerable<Lazy<CodeFixProvider, LanguageMetadata>>? exports = exportProvider.GetExports<CodeFixProvider, LanguageMetadata>();

        return exports.Where(export => export.Metadata.Languages.Contains(language)).Select(export => export.Value);
    }

    private class LanguageMetadata
    {
        public LanguageMetadata(IDictionary<string, object> data)
        {
            if (!data.TryGetValue(nameof(ExportCodeFixProviderAttribute.Languages), out var languages))
                languages = Array.Empty<string>();

            Languages = [..(string[])languages];
        }

        public ImmutableArray<string> Languages { get; }
    }
}