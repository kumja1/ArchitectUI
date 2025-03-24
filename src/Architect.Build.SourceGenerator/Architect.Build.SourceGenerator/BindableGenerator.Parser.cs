using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Architect.Build.SourceGenerator;

internal partial class BindableGenerator
{
    private static bool SyntaxProviderPredicate(
        SyntaxNode syntaxNode,
        CancellationToken cancellationToken
    )
    {
        return syntaxNode is PropertyDeclarationSyntax property && IsCandidateProperty(property);
    }

    private static PropertyInfo SyntaxProviderTransform(
        GeneratorSyntaxContext context,
        CancellationToken cancellationToken
    )
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (
            DoesReturnString(method, context.SemanticModel, cancellationToken)
            && HasHelloWorldAttribute(method, context.SemanticModel, cancellationToken)
        )
        {
            return method;
        }

        return null;
    }

    private static bool IsCandidateProperty(PropertyDeclarationSyntax method)
    {
        return method.AttributeLists.Count > 0
            && method.Modifiers.Count > 0
            && method.Modifiers.Any(SyntaxKind.PartialKeyword);
    }

    private static bool HasBindableAttribute(
        MethodDeclarationSyntax method,
        SemanticModel semanticModel,
        CancellationToken cancellationToken
    )
    {
        const string bindableAttributeName = "Roslyn.Generated.HelloWorldAttribute";

        foreach (AttributeListSyntax attributeList in method.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                if (
                    semanticModel.GetSymbolInfo(attribute, cancellationToken).Symbol
                    is IMethodSymbol attributeSymbol
                )
                {
                    string fullName = attributeSymbol.ContainingType.ToDisplayString();

                    if (fullName.Equals(bindableAttributeName, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
