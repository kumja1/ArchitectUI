using System.Text;
using Architect.Build.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Architect.Build.SourceGenerator;

[Generator]
internal sealed partial class BindableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitializationCallback);

        var supportsPartial = context.AnalyzerConfigOptionsProvider.Select(SupportsPartial);

        IncrementalValuesProvider<(ClassInfo, bool)> outputProvider = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "Roslyn.Generated.BindableObjectAttribute",
                SyntaxProviderPredicate,
                SyntaxProviderTransform
            )
            .Combine(supportsPartial);

        context.RegisterSourceOutput(outputProvider, SourceOutputAction);
    }

    private static void PostInitializationCallback(
        IncrementalGeneratorPostInitializationContext context
    )
    {
        context.AddSource(
            "BindableAttribute.g.cs",
            SourceText.From(bindableAttribute, Encoding.UTF8)
        );

        context.AddSource(
            "BindablePropertyAttribute.g.cs",
            SourceText.From(bindablePropertyAttribute, Encoding.UTF8)
        );
    }

    private static void SourceOutputAction(
        SourceProductionContext context,
        (ClassInfo Left, bool Right) candidate
    )
    {
        if (candidate.Left == ClassInfo.Empty)
            return;

        var source = GenerateSourceCode(candidate.Left, candidate.Right, context.CancellationToken);

        context.AddSource($"{candidate.Left.Name}.g.cs", source);
    }

    private static bool SupportsPartial(
        AnalyzerConfigOptionsProvider analyzerConfig,
        CancellationToken token
    )
    {
        token.ThrowIfCancellationRequested();

        if (
            !analyzerConfig.GlobalOptions.TryGetValue(
                "build_property.TargetFramework",
                out var targetFramework
            )
        )
            return false;

        string versionString = targetFramework.Replace("net", "").Replace("coreapp", "");

        if (Version.TryParse(versionString, out var version))
            return version.Major >= 9;
        return true;
    }
}
