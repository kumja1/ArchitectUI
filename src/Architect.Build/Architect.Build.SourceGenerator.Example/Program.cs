using Architect.Common.Interfaces;
using Roslyn.Generated;

namespace Architect.Build.SourceGenerator.Example;

[BindableObject]
public partial class BindableModel : IBindable
{
    [BindableProperty]
    private string monkey { get; set; }
}


public class Program
{
    public static void Main(string[] args)
    {
        var model = new BindableModel { Monkey = "Hello, World!" };
        Console.WriteLine(model.Monkey);
    }
}
