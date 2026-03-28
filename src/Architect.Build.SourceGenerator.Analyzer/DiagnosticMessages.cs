using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Architect.Build.SourceGenerator.Analyzer;

public static class DiagnosticMessages
{
    // public static readonly DiagnosticDescriptor ClassMustBePartial = new (
    //     id: "AG0001",
    //     title: "Class must be partial",
    //     messageFormat: "Class '{0}' must be partial",
    //     category: "Naming",
    //     defaultSeverity: DiagnosticSeverity.Error,
    //     isEnabledByDefault: true,
    //     description: "Class must be partial."
    // );

    public static readonly DiagnosticDescriptor ClassMustInheritBindableObject = new(
        id: "AG0002",
        title: "Class must inherit BindableObject",
        messageFormat: "Class '{0}' must inherit BindableObject",
        category: "Naming",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Class must inherit BindableObject."
    );

    // public static readonly DiagnosticDescriptor MemberMustBePartial = new (
    //     id: "AG0003",
    //     title: "Member must be partial",
    //     messageFormat: "Member '{0}' must be partial. This will be skipped by the source generator.",
    //     category: "Naming",
    //     defaultSeverity: DiagnosticSeverity.Error,
    //     isEnabledByDefault: true,
    //     description: "Member must be partial."
    // );

    public static readonly DiagnosticDescriptor MemberNameIsIdentical = new(
        id: "AG0004",
        title: "Property name is identical",
        messageFormat: "Property name '{0}' is identical to the generated property name",
        category: "Naming",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Property name is identical."
    );

    public static readonly DiagnosticDescriptor MemberCanBeField = new(
        id: "AG0005",
        title: "Member can be field",
        messageFormat: "Member '{0}' can be a field",
        category: "Naming",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Member can be a field."
    );

    public static readonly DiagnosticDescriptor MemberShouldBePrivate = new(
        id: "AG0006",
        title: "Member should be private",
        messageFormat: "Member '{0}' should be private",
        category: "Naming",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Member should be private."
    );

    public static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =
    [
        // ClassMustBePartial,
        ClassMustInheritBindableObject,
        // MemberMustBePartial,
        MemberNameIsIdentical,
        MemberCanBeField,
        MemberShouldBePrivate,
    ];
}