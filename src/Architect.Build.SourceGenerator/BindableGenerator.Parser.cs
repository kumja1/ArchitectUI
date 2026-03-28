using System.Diagnostics;
using Architect.Build.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Architect.Build.SourceGenerator;

internal partial class BindableGenerator
{
    private static bool SyntaxProviderPredicate(
        SyntaxNode syntaxNode,
        CancellationToken token
    )
    {
        token.ThrowIfCancellationRequested();
        bool isCandidate = IsCandidateClass(syntaxNode);
        if (isCandidate)
        {
            Debug.WriteLine(
                $"[BindableGenerator.SyntaxProviderPredicate] Found candidate class: {((ClassDeclarationSyntax)syntaxNode).Identifier.Text}");
        }

        return isCandidate;
    }

    private static bool IsCandidateClass(SyntaxNode syntaxNode)
    {
        Debug.WriteLine(
            $"[BindableGenerator.IsCandidateClass] IsCandidateClass checking: {syntaxNode.Span.ToString()}");
        bool isCandidate = syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclaration
                           && classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword);
        Debug.WriteLine($"[BindableGenerator.IsCandidateClass] IsCandidateClass result: {isCandidate}");
        return isCandidate;
    }

    private static ClassInfo SyntaxProviderTransform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token
    )
    {
        ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)context.TargetNode;
        Debug.WriteLine(
            $"[BindableGenerator.SyntaxProviderTransform] Transforming class: {classDeclaration.Identifier.Text}");
        BaseNamespaceDeclarationSyntax? namespaceDeclaration = classDeclaration
            .Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .First();

        List<MemberInfo> properties = [];
        if (!HasBindableObjectAttribute(classDeclaration, token))
        {
            Debug.WriteLine(
                $"[BindableGenerator.SyntaxProviderTransform] Class {classDeclaration.Identifier.Text} does not have BindableObjectAttribute.");
            return ClassInfo.Empty;
        }

        Debug.WriteLine(
            $"[BindableGenerator.SyntaxProviderTransform] Scanning members of {classDeclaration.Identifier.Text}");
        foreach (MemberDeclarationSyntax memberDeclaration in classDeclaration.Members)
        {
            if (memberDeclaration is not (PropertyDeclarationSyntax or FieldDeclarationSyntax))
            {
                Debug.WriteLine(
                    $"[BindableGenerator.SyntaxProviderTransform] Skipping non-property/field member: {memberDeclaration.GetType().Name}");
                continue;
            }

            if (
                !HasBindablePropertyAttribute(
                    memberDeclaration,
                    token
                )
            )
            {
                Debug.WriteLine(
                    $"[BindableGenerator.SyntaxProviderTransform] Member {memberDeclaration} does not have BindablePropertyAttribute.");
                continue;
            }

            Debug.WriteLine($"[BindableGenerator.SyntaxProviderTransform] Transforming member: {memberDeclaration}");
            if (memberDeclaration is FieldDeclarationSyntax fieldDeclaration)
            {
                foreach (VariableDeclaratorSyntax variable in fieldDeclaration.Declaration.Variables)
                {
                    Debug.WriteLine(
                        $"[BindableGenerator.SyntaxProviderTransform] Found variable in field: {variable.Identifier.Text}");
                    properties.Add(
                        TransformMember(
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
            {
                properties.Add(TransformMember(memberDeclaration));
            }
        }

        INamespaceSymbol? namespaceSymbol =
            context.SemanticModel.GetDeclaredSymbol(namespaceDeclaration, token)
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

    private static MemberInfo TransformMember(MemberDeclarationSyntax member)
    {
        string propertyName;
        string propertyType;
        string[] modifiers =
        [
            .. member switch
            {
                PropertyDeclarationSyntax prop => prop.Modifiers.Select(m => m.Text),
                FieldDeclarationSyntax field => field.Modifiers.Select(m => m.Text),
                _ => [],
            },
        ];

        bool isProperty = false;
        switch (member)
        {
            case PropertyDeclarationSyntax propDecl:
                isProperty = true;
                propertyName = propDecl.Identifier.ValueText;
                propertyType = propDecl.Type.ToString();
                break;
            case FieldDeclarationSyntax fieldDecl:
            {
                VariableDeclaratorSyntax? variable = fieldDecl.Declaration.Variables.FirstOrDefault();
                if (variable is null)
                    return MemberInfo.Empty;
                propertyName = variable.Identifier.ValueText;
                propertyType = fieldDecl.Declaration.Type.ToString();
                break;
            }
            default:
                return MemberInfo.Empty;
        }

        string backingFieldName = $"_{propertyName.ToLowerInvariant()}";
        return new MemberInfo(propertyName, propertyType, backingFieldName, isProperty, modifiers);
    }

    private static bool HasBindableObjectAttribute(
        ClassDeclarationSyntax classDeclaration,
        CancellationToken token
    )
    {
        token.ThrowIfCancellationRequested();
        const string bindableAttributeName = "Roslyn.Generated.BindableAttribute";
        foreach (AttributeListSyntax attributeList in classDeclaration.AttributeLists)
        {
            token.ThrowIfCancellationRequested();
            return attributeList.Attributes.Any(a =>
                string.Equals(a.Name.ToString(), bindableAttributeName, StringComparison.Ordinal));
        }

        return false;
    }

    private static bool HasBindablePropertyAttribute(
        MemberDeclarationSyntax member,
        CancellationToken token
    )
    {
        token.ThrowIfCancellationRequested();
        const string bindableAttributeName = "Roslyn.Generated.BindablePropertyAttribute";
        foreach (AttributeListSyntax attributeList in member.AttributeLists)
        {
            token.ThrowIfCancellationRequested();
            return attributeList.Attributes.Any(a =>
                string.Equals(a.Name.ToString(), bindableAttributeName, StringComparison.Ordinal));
        }

        return false;
    }
}