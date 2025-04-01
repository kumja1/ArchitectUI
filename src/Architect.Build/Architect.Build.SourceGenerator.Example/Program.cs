using Architect.UI.Data.Interfaces;
using Roslyn.Generated;

namespace Architect.Build.SourceGenerator.Example;

[BindableObject]
public partial class BindableModel : IBindable
{
    [BindableProperty]
    private string _text;
}






public class Program
{
    public static void Main(string[] args)
    {
        var model = new BindableModel { Text = "Hello, World!" };
        Console.WriteLine(model.Text);
    }
}
