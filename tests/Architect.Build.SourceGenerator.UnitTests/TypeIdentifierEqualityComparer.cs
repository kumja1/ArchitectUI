using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Architect.Build.SourceGenerator;

internal sealed class TypeIdentifierEqualityComparer : IEqualityComparer<TypeDeclarationSyntax?>
{
    public static TypeIdentifierEqualityComparer Instance { get; } =
        new TypeIdentifierEqualityComparer();

    private TypeIdentifierEqualityComparer() { }

    public bool Equals(TypeDeclarationSyntax? x, TypeDeclarationSyntax? y)
    {
        if (x is null)
        {
            return y is null;
        }

        return x.Identifier.ValueText.Equals(y?.Identifier.ValueText, StringComparison.Ordinal);
    }

    public int GetHashCode(TypeDeclarationSyntax? obj)
    {
        return obj?.Identifier.ValueText.GetHashCode() ?? 0;
    }
}
