using Architect.Common.Interfaces;
using Roslyn.Generated;

namespace Architect.Build.SourceGenerator.Example;

[BindableObject]
public partial class BindableModel : IBindable
{
    [BindableProperty]
    private string name;
}

public class Program
{
    public static void Main(string[] args)
    {
        var model = new BindableModel();

        model.PropertyChanged += (propertyName, value) =>
        {
            Console.WriteLine($"Property {propertyName} changed to {value}");
        };

        model.Name = "Hello, World!";
        model.Name = "Hello, World!";

        model.Name = "Hello, Universe!";
        model.Name = "Hello, Multiverse!";
    }
}
