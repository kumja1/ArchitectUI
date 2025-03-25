using System.Collections.Immutable;
using Architect.Build.SourceGenerator.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Architect.Build.SourceGenerator.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BindableAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AG";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        DiagnosticMessages.SupportedDiagnostics;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(CompilationStartAction);
    }

    private static void CompilationStartAction(CompilationStartAnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeBindableObject, SyntaxKind.Attribute);
    }

    private static void AnalyzeBindableObject(SyntaxNodeAnalysisContext context)
    {
        var attribute = (AttributeSyntax)context.Node;
        bool supportsPartial = Utils.CheckSupportsPartial(
            context.Options.AnalyzerConfigOptionsProvider
        );

        if (attribute.Name.ToString() != "BindableObject")
            return;

        var classDeclaration = attribute
            .Ancestors()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();

        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol is null)
            return;

        if (classDeclaration is null)
            return;

        if (
            classDeclaration.BaseList?.Types.Count > 0
            && classDeclaration.BaseList.Types.All(t => t.Type.ToString() != "IBindable")
        )
            ReportDiagnostic(
                context,
                DiagnosticMessages.ClassMustImplementIBindable,
                classDeclaration.Identifier.Text,
                classDeclaration
            );

        if (!classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword) && supportsPartial)
            ReportDiagnostic(
                context,
                DiagnosticMessages.ClassMustBePartial,
                classDeclaration.Identifier.Text,
                classDeclaration
            );

        foreach (var memberDeclaration in classDeclaration.Members)
        {
            if (memberDeclaration is not (PropertyDeclarationSyntax or FieldDeclarationSyntax))
                continue;

            if (
                memberDeclaration
                    .AttributeLists.SelectMany(a => a.Attributes)
                    .Any(a => a.Name.ToString() != "BindableProperty")
            )
                continue;

            AnalyzeBindableProperty(context, memberDeclaration, supportsPartial);
        }
    }

    private static void AnalyzeBindableProperty(
        SyntaxNodeAnalysisContext context,
        MemberDeclarationSyntax memberDeclaration,
        bool supportsPartial
    )
    {
        if (!memberDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword) && supportsPartial)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticMessages.MemberMustBePartial,
                    memberDeclaration.GetLocation(),
                    memberDeclaration.ToString()
                )
            );
        }

        if (memberDeclaration is PropertyDeclarationSyntax propertyDeclaration)
        {
            var name = propertyDeclaration.Identifier.Text;
            var generatedName = Utils.ToAlpha(name);
            if (name == generatedName)
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticMessages.MemberNameIsIdentical,
                        propertyDeclaration.GetLocation(),
                        propertyDeclaration.Identifier.Text
                    )
                );

            if (!supportsPartial && name == generatedName)
                ReportDiagnostic(
                    context,
                    DiagnosticMessages.MemberCanBeField,
                    propertyDeclaration.Identifier.Text,
                    propertyDeclaration
                );
        }
        else if (memberDeclaration is FieldDeclarationSyntax fieldDeclaration)
        {
            var name = fieldDeclaration.Declaration.Variables.First().Identifier.Text;
            var generatedName = Utils.ToAlpha(name);
            if (name == generatedName)
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticMessages.MemberNameIsIdentical,
                        fieldDeclaration.GetLocation(),
                        fieldDeclaration.Declaration.Variables.First().Identifier.Text
                    )
                );
        }
    }

    private static void ReportDiagnostic(
        SyntaxNodeAnalysisContext context,
        DiagnosticDescriptor rule,
        string message,
        SyntaxNode node
    ) => context.ReportDiagnostic(Diagnostic.Create(rule, node.GetLocation(), message));
}
