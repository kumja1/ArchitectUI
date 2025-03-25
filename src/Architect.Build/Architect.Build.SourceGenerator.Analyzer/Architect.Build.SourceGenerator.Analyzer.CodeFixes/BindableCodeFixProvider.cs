using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Architect.Build.SourceGenerator.Analyzer.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BindableCodeFixProvider)), Shared]
public sealed class BindableCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        [.. DiagnosticMessages.SupportedDiagnostics.Select(diagnostic => diagnostic.Id)];

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context
            .Document.GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);

        if (root is null)
            return;

        foreach (var diagnostic in context.Diagnostics)
        {
            if (!diagnostic.Id.StartsWith(BindableAnalyzer.DiagnosticId))
                continue;

            var node = root.FindNode(diagnostic.Location.SourceSpan);
            if (node is null)
                continue;

            switch (diagnostic.Id)
            {
                case nameof(DiagnosticMessages.ClassMustBePartial):
                    RegisterCodeFix(
                        context,
                        CodeActions.MakeClassPartialTitle,
                        cancellationToken =>
                            CodeActions.MakeMemberPartialAsync(
                                context.Document,
                                node,
                                cancellationToken
                            ),
                        diagnostic
                    );
                    break;
                case nameof(DiagnosticMessages.ClassMustImplementIBindable):
                    RegisterCodeFix(
                        context,
                        CodeActions.ImplementIBindableTitle,
                        cancellationToken =>
                            CodeActions.ImplementIBindableAsync(
                                context.Document,
                                node,
                                cancellationToken
                            ),
                        diagnostic
                    );
                    break;
                case nameof(DiagnosticMessages.MemberMustBePartial):
                    RegisterCodeFix(
                        context,
                        CodeActions.MakeMemberPartialTitle,
                        cancellationToken =>
                            CodeActions.MakeMemberPartialAsync( // Ensure this method is implemented in CodeActions
                                context.Document,
                                node,
                                cancellationToken
                            ),
                        diagnostic
                    );
                    break;
                case nameof(DiagnosticMessages.MemberNameIsIdentical):
                    RegisterCodeFix(
                        context,
                        CodeActions.RenameMemberTitle,
                        cancellationToken =>
                            CodeActions.RenameMemberAsync(
                                context.Document,
                                node,
                                cancellationToken
                            ),
                        diagnostic
                    );
                    break;
                case nameof(DiagnosticMessages.MemberCanBeField):
                    RegisterCodeFix(
                        context,
                        CodeActions.MakeMemberFieldTitle,
                        cancellationToken =>
                            CodeActions.MakeMemberFieldAsync(
                                context.Document,
                                node,
                                cancellationToken
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
