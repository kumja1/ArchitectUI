using System.Reflection;

namespace Architect.Build.SourceGenerator.Models;

internal readonly record struct ClassInfo(
    string Name,
    NamespaceInfo Namespace,
    EquatableArray<MemberInfo> Members,
    EquatableArray<string> Modifiers
)
{
    public static ClassInfo Empty =>
        new(string.Empty, NamespaceInfo.Empty, [], []);
}