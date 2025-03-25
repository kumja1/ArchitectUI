namespace Architect.Build.SourceGenerator.Models;

internal readonly record struct PropertyInfo(
    string Name,
    string Type,
    string BackingFieldName,
    IReadOnlyList<string> Modifiers,
    bool IsPartial = false
)
{
    public static PropertyInfo Empty => new(string.Empty, string.Empty, string.Empty, []);
}
