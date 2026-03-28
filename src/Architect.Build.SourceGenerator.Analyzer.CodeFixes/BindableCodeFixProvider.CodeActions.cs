using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Architect.Build.SourceGenerator.Analyzer.CodeFixes;

partial class BindableCodeFixProvider
{
    // Code action titles
    // public const string MakeClassPartialTitle = "Make class partial";
    private const string INHERIT_BINDABLE_OBJECT_TITLE = "Extend BindableObject";
    private const string RENAME_MEMBER_TITLE = "Rename member";
    private const string MAKE_MEMBER_FIELD_TITLE = "Make member a field";
    // public const string MakeMemberPartialTitle = "Make member partial";

    // public static async Task<Solution> MakeMemberPartialAsync(
    //     Document document,
    //     SyntaxNode node,
    //     CancellationToken cancellationToken
    // )
    // {
    //     if (node is not MemberDeclarationSyntax member)
    //         return document.Project.Solution;
    //
    //     DocumentEditor? editor = await DocumentEditor
    //         .CreateAsync(document, cancellationToken)
    //         .ConfigureAwait(false);
    //
    //     member = member.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
    //     editor.ReplaceNode(node, member);
    //     return editor.GetChangedDocument().Project.Solution;
    // }

    private static async Task<Solution> ExtendBindableObjectAsync(
        Document document,
        SyntaxNode node,
        CancellationToken cancellationToken
    )
    {
        Debug.WriteLine($"Applying ExtendBindableObjectAsync code fix to {document.Name}");
        if (node is not ClassDeclarationSyntax classDeclaration)
        {
            Debug.WriteLine("Node is not ClassDeclarationSyntax, skipping fix.");
            return document.Project.Solution;
        }

        DocumentEditor? editor = await DocumentEditor
            .CreateAsync(document, cancellationToken)
            .ConfigureAwait(false);

        SyntaxNode className = editor.Generator.IdentifierName("BindableObject");
        editor.AddBaseType(classDeclaration, className);
        return editor.GetChangedDocument().Project.Solution;
    }

    private static async Task<Solution> RenameMemberAsync(
        Document document,
        SyntaxNode node,
        CancellationToken cancellationToken
    )
    {
        Debug.WriteLine($"Applying RenameMemberAsync code fix to {document.Name}");
        if (node is not MemberDeclarationSyntax memberDeclaration)
        {
            Debug.WriteLine("Node is not MemberDeclarationSyntax, skipping fix.");
            return document.Project.Solution;
        }

        DocumentEditor? editor = await DocumentEditor
            .CreateAsync(document, cancellationToken)
            .ConfigureAwait(false);

        SemanticModel? semanticModel = await document
            .GetSemanticModelAsync(cancellationToken)
            .ConfigureAwait(false);

        ISymbol? symbol = semanticModel.GetDeclaredSymbol(memberDeclaration, cancellationToken);
        if (symbol == null)
            return document.Project.Solution;

        string newName = $"_{symbol.Name.ToLowerInvariant()}";
        editor.SetName(memberDeclaration, newName);

        return editor.GetChangedDocument().Project.Solution;
    }

    private static async Task<Solution> MakeMemberFieldAsync(
        Document document,
        SyntaxNode node,
        CancellationToken cancellationToken
    )
    {
        Debug.WriteLine("Applying MakeMemberFieldAsync code fix");
        if (node is not PropertyDeclarationSyntax propertyDeclaration)
            return document.Project.Solution;

        DocumentEditor? editor = await DocumentEditor
            .CreateAsync(document, cancellationToken)
            .ConfigureAwait(false);

        SemanticModel? semanticModel = await document
            .GetSemanticModelAsync(cancellationToken)
            .ConfigureAwait(false);

        IPropertySymbol? symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

        if (symbol == null)
            return document.Project.Solution;

        SyntaxGenerator syntaxGenerator = editor.Generator;
        SyntaxNode fieldDeclaration = syntaxGenerator.FieldDeclaration(
            propertyDeclaration.Identifier.Text,
            propertyDeclaration.Type,
            symbol.DeclaredAccessibility,
            DeclarationModifiers.From(symbol),
            propertyDeclaration.Initializer
        );

        editor.ReplaceNode(propertyDeclaration, fieldDeclaration);
        return editor.GetChangedDocument().Project.Solution;
    }
}