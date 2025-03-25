namespace Architect.Build.SourceGenerator.Models;

internal readonly record struct ClassInfo(
    string Name,
    NamespaceInfo Namespace,
    EquatableArray<PropertyInfo> Properties,
    EquatableArray<string> Modifiers
)
{
    public static ClassInfo Empty =>
        new(string.Empty, NamespaceInfo.Empty, Array.Empty<PropertyInfo>(), Array.Empty<string>());
}
