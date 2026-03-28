using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Architect.Build.SourceGenerator.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BindableAnalyzer : DiagnosticAnalyzer
{
    private static readonly TextInfo _textInfo = CultureInfo.InvariantCulture.TextInfo;

    public const string DIAGNOSTIC_ID = "AG";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        DiagnosticMessages.SupportedDiagnostics;


    public override void Initialize(AnalysisContext context)
    {
        Debug.WriteLine("Initializing BindableAnalyzer...");
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(CompilationStartAction);
    }

    private static void CompilationStartAction(CompilationStartAnalysisContext context)
    {
        Debug.WriteLine("CompilationStartAction triggered.");
        context.RegisterSyntaxNodeAction(AnalyzeBindableObject, SyntaxKind.Attribute);
    }

    private static void AnalyzeBindableObject(SyntaxNodeAnalysisContext context)
    {
        Debug.WriteLine("AnalyzeBindableObject triggered.");
        AttributeSyntax attribute = (AttributeSyntax)context.Node;
        Debug.WriteLine($"Analyzing attribute: {attribute.Name}. Node path: {attribute.SyntaxTree.FilePath}");

        // bool supportsPartial = CheckSupportsPartial(
        //     context.Options.AnalyzerConfigOptionsProvider.
        // );
        // Debug.WriteLine($"SupportsPartial: {supportsPartial}");

        Debug.WriteLine("Attribute name: " + attribute.Name + "");
        if (attribute.Name.ToString() != "Bindable")
        {
            Debug.WriteLine("Attribute is not Bindable. Skipping.");
            return;
        }

        ClassDeclarationSyntax? classDeclaration = attribute
            .Ancestors()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();

        if (classDeclaration is null)
        {
            Debug.WriteLine("ClassDeclaration is null. Skipping.");
            return;
        }

        INamedTypeSymbol? classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol is null)
        {
            Debug.WriteLine("ClassSymbol is null. Skipping.");
            return;
        }

        Debug.WriteLine($"Analyzing class: {classDeclaration.Identifier.Text}");

        bool inheritsBindableObject = classDeclaration.BaseList?.Types
            .Any(t => t.Type.ToString() == "BindableObject") ?? false;

        if (!inheritsBindableObject)
        {
            Debug.WriteLine("Class does not inherit BindableObject.");
            ReportDiagnostic(
                context,
                DiagnosticMessages.ClassMustInheritBindableObject,
                classDeclaration,
                classDeclaration.Identifier.Text
            );
        }

        // if (!classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        // {
        //     Debug.WriteLine("Class is not partial.");
        //     ReportDiagnostic(
        //         context,
        //         DiagnosticMessages.ClassMustBePartial,
        //         classDeclaration,
        //         classDeclaration.Identifier.Text
        //     );
        // }

        foreach (MemberDeclarationSyntax memberDeclaration in classDeclaration.Members)
        {
            Debug.WriteLine($"Analyzing member: {memberDeclaration}");
            if (memberDeclaration is not (PropertyDeclarationSyntax or FieldDeclarationSyntax))
            {
                Debug.WriteLine("Member is not a property or field. Skipping.");
                continue;
            }

            if (memberDeclaration.AttributeLists.SelectMany(a => a.Attributes).All(a => a.Name.ToString() != "BindableProperty"))
            {
                Debug.WriteLine("Member does not have BindableProperty attribute. Skipping.");
                continue;
            }

            // AnalyzeBindableProperty(context, memberDeclaration, supportsPartial);
            AnalyzeBindableProperty(context, memberDeclaration);
        }
    }

    private static void AnalyzeBindableProperty(
        SyntaxNodeAnalysisContext context,
        MemberDeclarationSyntax memberDeclaration
        // bool supportsPartial
    )
    {
        Debug.WriteLine($"AnalyzeBindableProperty triggered for member: {memberDeclaration}");
        // if (!memberDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword) && supportsPartial)
        // {
        //     Debug.WriteLine("Member is not partial.");
        //     ReportDiagnostic(
        //             context,
        //             DiagnosticMessages.MemberMustBePartial,
        //             memberDeclaration,
        //             memberDeclaration.ToString()
        //         );
        // }

        switch (memberDeclaration)
        {
            case PropertyDeclarationSyntax propertyDeclaration:
            {
                string name = propertyDeclaration.Identifier.Text;
                var generatedName = _textInfo.ToTitleCase(
                    // supportsPartial ? name : name.TrimStart('_')
                    memberDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword) ? name : name.TrimStart('_')
                );

                Debug.WriteLine($"Property name: {name}, Generated name: {generatedName}");

                if (name == generatedName)
                {
                    Debug.WriteLine("Property name is identical to generated name.");
                    ReportDiagnostic(
                        context,
                        DiagnosticMessages.MemberNameIsIdentical,
                        propertyDeclaration,
                        propertyDeclaration.Identifier.Text
                    );
                }

                if (!propertyDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    Debug.WriteLine("Property can be a field.");
                    ReportDiagnostic(
                        context,
                        DiagnosticMessages.MemberCanBeField,
                        propertyDeclaration,
                        propertyDeclaration.Identifier.Text
                    );
                }

                break;
            }
            case FieldDeclarationSyntax fieldDeclaration:
            {
                string name = fieldDeclaration.Declaration.Variables.First().Identifier.Text;
                var generatedName = _textInfo.ToTitleCase(name[1..]);
                Debug.WriteLine($"Field name: {name}, Generated name: {generatedName}");

                if (name == generatedName)
                {
                    Debug.WriteLine("Field name is identical to generated name.");
                    ReportDiagnostic(
                        context,
                        DiagnosticMessages.MemberNameIsIdentical,
                        fieldDeclaration,
                        fieldDeclaration.Declaration.Variables.First().Identifier.Text
                    );
                }

                break;
            }
        }

        if (memberDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword))
            return;

        Debug.WriteLine("Member is not private.");
        ReportDiagnostic(
            context,
            DiagnosticMessages.MemberShouldBePrivate,
            memberDeclaration,
            memberDeclaration.ToString()
        );
    }

    private static void ReportDiagnostic(
        SyntaxNodeAnalysisContext context,
        DiagnosticDescriptor rule,
        SyntaxNode node,
        params object[] args
    )
    {
        Debug.WriteLine($"Reporting diagnostic: {rule.Id} at {node.GetLocation().GetLineSpan()}");
        context.ReportDiagnostic(Diagnostic.Create(rule, node.GetLocation(), args));
    }
}