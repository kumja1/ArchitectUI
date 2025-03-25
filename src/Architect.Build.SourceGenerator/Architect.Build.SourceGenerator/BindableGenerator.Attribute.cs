using System.Reflection;

namespace Architect.Build.SourceGenerator;

internal partial class BindableGenerator
{
    private static readonly AssemblyName assemblyName =
        typeof(BindableGenerator).Assembly.GetName();
    private static readonly string generatedCodeAttribute =
        $@"global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{assemblyName.Name}"", ""{assemblyName.Version}"")";

    private static readonly string bindableAttribute =
        $@"// <auto-generated/>
#nullable enable

namespace Roslyn.Generated;

	[{generatedCodeAttribute}]
	[global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false)]
	internal sealed class BindableObjectAttribute : global::System.Attribute;
";

    private static readonly string bindablePropertyAttribute =
        $@"// <auto-generated/>
#nullable enable

namespace Roslyn.Generated;

	[{generatedCodeAttribute}]
	[global::System.AttributeUsage(global::System.AttributeTargets.Property | global::System.AttributeTargets.Field  , AllowMultiple = false)]
	internal sealed class BindablePropertyAttribute : global::System.Attribute;
";
}
