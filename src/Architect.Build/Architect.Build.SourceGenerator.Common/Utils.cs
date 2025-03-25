using Microsoft.CodeAnalysis.Diagnostics;

namespace Architect.Build.SourceGenerator.Common;

public static class Utils
{
    public static bool CheckSupportsPartial(AnalyzerConfigOptionsProvider analyzerConfig)
    {
        if (
            !analyzerConfig.GlobalOptions.TryGetValue(
                "build_property.TargetFramework",
                out var targetFramework
            )
        )
            return false;

        string versionString = targetFramework.Replace("net", "").Replace("coreapp", "");

        if (Version.TryParse(versionString, out var version))
            return version.Major >= 9;
        return true;
    }

    public static string ToAlpha(string s) => char.ToUpperInvariant(s[0]) + s[1..];
}
