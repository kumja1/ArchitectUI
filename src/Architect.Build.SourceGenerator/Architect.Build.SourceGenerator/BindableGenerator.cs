using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Architect.Build.SourceGenerator;

[Generator(LanguageNames.CSharp)]
internal sealed partial class BindableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitializationCallback);

        IncrementalValueProvider<ImmutableArray<MethodDeclarationSyntax>> syntaxProvider = context
            .SyntaxProvider.CreateSyntaxProvider(SyntaxProviderPredicate, SyntaxProviderTransform)
            .Where(static method => method is not null)
            .Collect()!;

        IncrementalValueProvider<(
            ImmutableArray<MethodDeclarationSyntax> Left,
            Compilation Right
        )> source = syntaxProvider.Combine(context.CompilationProvider);

        context.RegisterSourceOutput(source, SourceOutputAction);
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
        (ImmutableArray<MethodDeclarationSyntax> Left, Compilation Right) candidates
    )
    {
        if (candidates.Left.IsEmpty)
        {
            return;
        }

        foreach (
            (string typeName, string source) in GenerateSourceCode(
                candidates.Left,
                candidates.Right,
                context.CancellationToken
            )
        )
        {
            context.AddSource($"{typeName}.HelloWorld.g.cs", source);
        }
    }
}
