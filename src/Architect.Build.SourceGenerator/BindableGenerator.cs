using System.Diagnostics;
using System.Text;
using Architect.Build.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Architect.Build.SourceGenerator;

[Generator]
internal sealed partial class BindableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Debug.WriteLine("[BindableGenerator.Initialize] Initializing BindableGenerator");
        context.RegisterPostInitializationOutput(PostInitializationCallback);

        // IncrementalValueProvider<bool> supportsPartial = context.AnalyzerConfigOptionsProvider.Select(SupportsPartial);
        // IncrementalValuesProvider<(ClassInfo, bool)> outputProvider = context
        //     .SyntaxProvider.ForAttributeWithMetadataName(
        //         "Roslyn.Generated.BindableObjectAttribute",
        //         SyntaxProviderPredicate,
        //         SyntaxProviderTransform
        //     ).Combine(supportsPartial);

        IncrementalValuesProvider<ClassInfo> outputProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            "Roslyn.Generated.BindableAttribute",
            SyntaxProviderPredicate,
            SyntaxProviderTransform
        );
        
        context.RegisterSourceOutput(outputProvider, SourceOutputAction);
    }

    private static void PostInitializationCallback(
        IncrementalGeneratorPostInitializationContext context
    )
    {
        context.AddSource(
            "BindableAttribute.g.cs",
            SourceText.From(_bindableAttribute, Encoding.UTF8)
        );

        context.AddSource(
            "BindablePropertyAttribute.g.cs",
            SourceText.From(_bindablePropertyAttribute, Encoding.UTF8)
        );
    }

    private static void SourceOutputAction(
        SourceProductionContext context,
        ClassInfo candidate
        // (ClassInfo Left, bool Right) candidate
    )
    {
        Debug.WriteLine($"[BindableGenerator.SourceOutputAction] SourceOutputAction started for {candidate.Name}");

        // if (candidate.Left == ClassInfo.Empty)
        //     return;

        if (candidate == ClassInfo.Empty)
        {
            Debug.WriteLine("[BindableGenerator.SourceOutputAction] Candidate is empty, returning.");
            return;
        }

        // string source = GenerateSourceCode(candidate.Left, candidate.Right, context.CancellationToken);
        // context.AddSource($"{candidate.Left.Name}.g.cs", source);

        string source = GenerateSourceCode(candidate, context.CancellationToken);
        Debug.WriteLine($"[BindableGenerator.SourceOutputAction] Source code generated for {candidate.Name}:");
        Debug.WriteLine($"[BindableGenerator.SourceOutputAction] {source}");
        context.AddSource($"{candidate.Name}.g.cs", source);
    }

    // private static bool SupportsPartial(
    //     AnalyzerConfigOptionsProvider analyzerConfig,
    //     CancellationToken token
    // )
    // {
    //     token.ThrowIfCancellationRequested();
    //     return CheckSupportsPartial(analyzerConfig);
    //
    //     static bool CheckSupportsPartial(AnalyzerConfigOptionsProvider analyzerConfig)
    //     {
    //         if (
    //             !analyzerConfig.GlobalOptions.TryGetValue(
    //                 "build_property.TargetFramework",
    //                 out string? targetFramework
    //             )
    //         )
    //             return false;
    //
    //         string versionString = targetFramework.Replace("net", "").Replace("coreapp", "");
    //         if (Version.TryParse(versionString, out Version? version))
    //             return version.Major >= 9;
    //         return true;
    //     }
    // }
}