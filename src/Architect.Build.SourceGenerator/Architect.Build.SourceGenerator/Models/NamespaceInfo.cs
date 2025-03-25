namespace Architect.Build.SourceGenerator.Models;

internal readonly record struct NamespaceInfo(string Name, bool IsGlobal)
{
    public static NamespaceInfo Empty => new(string.Empty, false);
}
