using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Architect.Build.SourceGenerator.Analyzer.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BindableCodeFixProvider)), Shared]
public sealed partial class BindableCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        [.. DiagnosticMessages.SupportedDiagnostics.Select(diagnostic => diagnostic.Id)];

    public override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode? root = await context
            .Document.GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);

        if (root is null)
            return;

        foreach (Diagnostic? diagnostic in context.Diagnostics)
        {
            if (!diagnostic.Id.StartsWith(BindableAnalyzer.DIAGNOSTIC_ID))
                continue;

            SyntaxNode? node = root.FindNode(diagnostic.Location.SourceSpan);
            if (node is null)
                continue;

            switch (diagnostic.Id)
            {
                case "AG0002":
                    RegisterCodeFix(
                        context,
                        INHERIT_BINDABLE_OBJECT_TITLE,
                        token =>
                            ExtendBindableObjectAsync(
                                context.Document,
                                node,
                                token
                            ),
                        diagnostic
                    );
                    break;
                case "AG0004":
                    RegisterCodeFix(
                        context,
                        RENAME_MEMBER_TITLE,
                        token =>
                            RenameMemberAsync(
                                context.Document,
                                node,
                                token
                            ),
                        diagnostic
                    );
                    break;
                case "AG0005":
                    RegisterCodeFix(
                        context,
                        MAKE_MEMBER_FIELD_TITLE,
                        token =>
                            MakeMemberFieldAsync(
                                context.Document,
                                node,
                                token
                            ),
                        diagnostic
                    );
                    break;
            }
        }
    }

    private static void RegisterCodeFix(
        CodeFixContext context,
        string title,
        Func<CancellationToken, Task<Solution>> createChangedSolution,
        Diagnostic diagnostic
    ) =>
        context.RegisterCodeFix(CodeAction.Create(title, createChangedSolution, title), diagnostic);
}