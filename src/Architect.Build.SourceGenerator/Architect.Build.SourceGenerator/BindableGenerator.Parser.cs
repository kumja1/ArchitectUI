using System.Text;
using Architect.Build.SourceGenerator.Models;
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
        cancellationToken.ThrowIfCancellationRequested();
        return IsCandidateClass(syntaxNode);
    }

    private static bool IsCandidateClass(SyntaxNode syntaxNode)
    {
        return syntaxNode is ClassDeclarationSyntax classDeclaration
            && classDeclaration.AttributeLists.Count > 0
            && classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)
            && classDeclaration?.BaseList?.Types.Count > 0
            && classDeclaration.BaseList.Types.Any(t =>
                t.Type is IdentifierNameSyntax identifierName
                && identifierName.Identifier.Text == "IBindable"
            );
    }

    private static ClassInfo SyntaxProviderTransform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken
    )
    {
        var classDeclaration = (ClassDeclarationSyntax)context.TargetNode;
        var namespaceDeclaration = classDeclaration
            .Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .First();

        var properties = new List<PropertyInfo>();
        if (!HasBindableObjectAttribute(classDeclaration, context.SemanticModel, cancellationToken))
            return ClassInfo.Empty;

        foreach (var memberDeclaration in classDeclaration.Members)
        {
            if (memberDeclaration is not (PropertyDeclarationSyntax or FieldDeclarationSyntax))
                continue;

            if (
                !HasBindablePropertyAttribute(
                    memberDeclaration,
                    context.SemanticModel,
                    cancellationToken
                )
            )
                continue;

            if (memberDeclaration is FieldDeclarationSyntax fieldDeclaration)
            {
                foreach (var variable in fieldDeclaration.Declaration.Variables)
                {
                    properties.Add(
                        TransformProperty(
                            fieldDeclaration.WithDeclaration(
                                fieldDeclaration.Declaration.WithVariables(
                                    SyntaxFactory.SingletonSeparatedList(variable)
                                )
                            )
                        )
                    );
                }
            }
            else
                properties.Add(TransformProperty(memberDeclaration));
        }

        var namespaceSymbol =
            context.SemanticModel.GetDeclaredSymbol(namespaceDeclaration, cancellationToken)
            as INamespaceSymbol;

        return new ClassInfo(
            classDeclaration.Identifier.ValueText,
            new NamespaceInfo(
                namespaceDeclaration.Name.ToString(),
                namespaceSymbol?.IsGlobalNamespace ?? false
            ),
            properties,
            classDeclaration.Modifiers.Select(m => m.Text).ToList()
        );
    }

    private static PropertyInfo TransformProperty(MemberDeclarationSyntax member)
    {
        string propertyName;
        string propertyType;
        List<string> modifiers =
        [
            .. member switch
            {
                PropertyDeclarationSyntax prop => prop.Modifiers.Select(m => m.Text),
                FieldDeclarationSyntax field => field.Modifiers.Select(m => m.Text),
                _ => [],
            },
        ];

        if (member is PropertyDeclarationSyntax propDecl)
        {
            propertyName = propDecl.Identifier.ValueText;
            propertyType = propDecl.Type.ToString();
        }
        else if (member is FieldDeclarationSyntax fieldDecl)
        {
            var variable = fieldDecl.Declaration.Variables.FirstOrDefault();
            if (variable is null)
                return PropertyInfo.Empty;
            propertyName = variable.Identifier.ValueText;
            propertyType = fieldDecl.Declaration.Type.ToString();
        }
        else
            return PropertyInfo.Empty;

        var backingFieldName = $"_{propertyName.ToLowerInvariant()}";
        return new PropertyInfo(propertyName, propertyType, backingFieldName, modifiers);
    }

    private static bool HasBindableObjectAttribute(
        ClassDeclarationSyntax classDeclaration,
        SemanticModel semanticModel,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string bindableAttributeName = "Roslyn.Generated.BindableObjectAttribute";
        foreach (AttributeListSyntax attributeList in classDeclaration.AttributeLists)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (
                    semanticModel.GetSymbolInfo(attribute, cancellationToken).Symbol
                    is IMethodSymbol attributeSymbol
                )
                {
                    string fullName = attributeSymbol.ContainingType.ToDisplayString();
                    return fullName.Equals(bindableAttributeName, StringComparison.Ordinal);
                }
            }
        }

        return false;
    }

    private static bool HasBindablePropertyAttribute(
        MemberDeclarationSyntax member,
        SemanticModel semanticModel,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string bindableAttributeName = "Roslyn.Generated.BindablePropertyAttribute";
        foreach (AttributeListSyntax attributeList in member.AttributeLists)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (
                    semanticModel.GetSymbolInfo(attribute, cancellationToken).Symbol
                    is IMethodSymbol attributeSymbol
                )
                {
                    string fullName = attributeSymbol.ContainingType.ToDisplayString();
                    return fullName.Equals(bindableAttributeName, StringComparison.Ordinal);
                }
            }
        }

        return false;
    }
}
