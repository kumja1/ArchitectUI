namespace Architect.Build.SourceGenerator.Models;

internal readonly record struct MemberInfo(
    string Name,
    string Type,
    string BackingFieldName,
    bool IsProperty,
    string[] Modifiers
)
{
    public static MemberInfo Empty => new(string.Empty, string.Empty, string.Empty, false, []);
}