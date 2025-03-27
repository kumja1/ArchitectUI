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
        Console.WriteLine("Initializing BindableAnalyzer...");
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(CompilationStartAction);
    }

    private static void CompilationStartAction(CompilationStartAnalysisContext context)
    {
        Console.WriteLine("CompilationStartAction triggered.");
        context.RegisterSyntaxNodeAction(AnalyzeBindableObject, SyntaxKind.Attribute);
    }

    private static void AnalyzeBindableObject(SyntaxNodeAnalysisContext context)
    {
        Console.WriteLine("AnalyzeBindableObject triggered.");
        var attribute = (AttributeSyntax)context.Node;
        Console.WriteLine($"Analyzing attribute: {attribute.Name}");

        bool supportsPartial = Utils.CheckSupportsPartial(
            context.Options.AnalyzerConfigOptionsProvider
        );
        Console.WriteLine($"SupportsPartial: {supportsPartial}");

        if (attribute.Name.ToString() != "BindableObject")
        {
            Console.WriteLine("Attribute is not BindableObject. Skipping.");
            return;
        }

        var classDeclaration = attribute
            .Ancestors()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();

        if (classDeclaration is null)
        {
            Console.WriteLine("ClassDeclaration is null. Skipping.");
            return;
        }

        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol is null)
        {
            Console.WriteLine("ClassSymbol is null. Skipping.");
            return;
        }

        Console.WriteLine($"Analyzing class: {classDeclaration.Identifier.Text}");

        if (
            classDeclaration.BaseList?.Types.Count > 0
            && classDeclaration.BaseList.Types.All(t => t.Type.ToString() != "IBindable")
        )
        {
            Console.WriteLine("Class does not implement IBindable.");
            ReportDiagnostic(
                context,
                DiagnosticMessages.ClassMustImplementIBindable,
                classDeclaration.Identifier.Text,
                classDeclaration
            );
        }

        if (!classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            Console.WriteLine("Class is not partial.");
            ReportDiagnostic(
                context,
                DiagnosticMessages.ClassMustBePartial,
                classDeclaration.Identifier.Text,
                classDeclaration
            );
        }

        foreach (var memberDeclaration in classDeclaration.Members)
        {
            Console.WriteLine($"Analyzing member: {memberDeclaration}");
            if (memberDeclaration is not (PropertyDeclarationSyntax or FieldDeclarationSyntax))
            {
                Console.WriteLine("Member is not a property or field. Skipping.");
                continue;
            }

            if (
                memberDeclaration
                    .AttributeLists.SelectMany(a => a.Attributes)
                    .Any(a => a.Name.ToString() != "BindableProperty")
            )
            {
                Console.WriteLine("Member does not have BindableProperty attribute. Skipping.");
                continue;
            }

            AnalyzeBindableProperty(context, memberDeclaration, supportsPartial);
        }
    }

    private static void AnalyzeBindableProperty(
        SyntaxNodeAnalysisContext context,
        MemberDeclarationSyntax memberDeclaration,
        bool supportsPartial
    )
    {
        Console.WriteLine($"AnalyzeBindableProperty triggered for member: {memberDeclaration}");

        if (!memberDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword) && supportsPartial)
        {
            Console.WriteLine("Member is not partial.");
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
            Console.WriteLine($"Property name: {name}, Generated name: {generatedName}");

            if (name == generatedName)
            {
                Console.WriteLine("Property name is identical to generated name.");
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticMessages.MemberNameIsIdentical,
                        propertyDeclaration.GetLocation(),
                        propertyDeclaration.Identifier.Text
                    )
                );
            }

            if (!supportsPartial)
            {
                Console.WriteLine("Property can be a field.");
                ReportDiagnostic(
                    context,
                    DiagnosticMessages.MemberCanBeField,
                    propertyDeclaration.Identifier.Text,
                    propertyDeclaration
                );
            }
        }
        else if (memberDeclaration is FieldDeclarationSyntax fieldDeclaration)
        {
            var name = fieldDeclaration.Declaration.Variables.First().Identifier.Text;
            var generatedName = Utils.ToAlpha(name);
            Console.WriteLine($"Field name: {name}, Generated name: {generatedName}");

            if (name == generatedName)
            {
                Console.WriteLine("Field name is identical to generated name.");
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticMessages.MemberNameIsIdentical,
                        fieldDeclaration.GetLocation(),
                        fieldDeclaration.Declaration.Variables.First().Identifier.Text
                    )
                );
            }
        }

        if (!memberDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword))
        {
            Console.WriteLine("Member is not private.");
            ReportDiagnostic(
                context,
                DiagnosticMessages.MemberShouldBePrivate,
                memberDeclaration.ToString(),
                memberDeclaration
            );
        }
    }

    private static void ReportDiagnostic(
        SyntaxNodeAnalysisContext context,
        DiagnosticDescriptor rule,
        string message,
        SyntaxNode node
    )
    {
        Console.WriteLine($"Reporting diagnostic: {rule.Id}, Message: {message}");
        context.ReportDiagnostic(Diagnostic.Create(rule, node.GetLocation(), message));
    }
}
