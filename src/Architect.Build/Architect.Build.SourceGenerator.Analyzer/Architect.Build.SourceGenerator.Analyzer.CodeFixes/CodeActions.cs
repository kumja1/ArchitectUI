using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Architect.Build.SourceGenerator.Analyzer.CodeFixes;

public static class CodeActions
{
    // Code action titles
    public const string MakeClassPartialTitle = "Make class partial";
    public const string ImplementIBindableTitle = "Implement IBindable";
    public const string RenameMemberTitle = "Rename member";
    public const string MakeMemberFieldTitle = "Make member a field";
    public const string MakeMemberPartialTitle = "Make member partial";

    public static async Task<Solution> MakeMemberPartialAsync(
        Document document,
        SyntaxNode node,
        CancellationToken cancellationToken
    )
    {
        if (node is not MemberDeclarationSyntax member)
            return document.Project.Solution;

        var editor = await DocumentEditor
            .CreateAsync(document, cancellationToken)
            .ConfigureAwait(false);

        member = member.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        editor.ReplaceNode(node, member);
        return editor.GetChangedDocument().Project.Solution;
    }

    public static async Task<Solution> ImplementIBindableAsync(
        Document document,
        SyntaxNode node,
        CancellationToken cancellationToken
    )
    {
        if (node is not ClassDeclarationSyntax classDeclaration)
            return document.Project.Solution;

        var editor = await DocumentEditor
            .CreateAsync(document, cancellationToken)
            .ConfigureAwait(false);

        var interfaceName = editor.Generator.IdentifierName("IBindable");
        editor.AddInterfaceType(classDeclaration, interfaceName);
        return editor.GetChangedDocument().Project.Solution;
    }

    public static async Task<Solution> RenameMemberAsync(
        Document document,
        SyntaxNode node,
        CancellationToken cancellationToken
    )
    {
        if (node is not MemberDeclarationSyntax memberDeclaration)
            return document.Project.Solution;

        var editor = await DocumentEditor
            .CreateAsync(document, cancellationToken)
            .ConfigureAwait(false);

        var semanticModel = await document
            .GetSemanticModelAsync(cancellationToken)
            .ConfigureAwait(false);

        var symbol = semanticModel.GetDeclaredSymbol(memberDeclaration, cancellationToken);
        if (symbol == null)
            return document.Project.Solution;

        var newName = $"_{symbol.Name.ToLowerInvariant()}";
        editor.SetName(memberDeclaration, newName);

        return editor.GetChangedDocument().Project.Solution;
    }

    public static async Task<Solution> MakeMemberFieldAsync(
        Document document,
        SyntaxNode node,
        CancellationToken cancellationToken
    )
    {
        if (node is not PropertyDeclarationSyntax propertyDeclaration)
            return document.Project.Solution;

        var editor = await DocumentEditor
            .CreateAsync(document, cancellationToken)
            .ConfigureAwait(false);

        var semanticModel = await document
            .GetSemanticModelAsync(cancellationToken)
            .ConfigureAwait(false);

        var symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

        if (symbol == null)
            return document.Project.Solution;

        var syntaxGenerator = editor.Generator;

        var fieldDeclaration = syntaxGenerator.FieldDeclaration(
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
