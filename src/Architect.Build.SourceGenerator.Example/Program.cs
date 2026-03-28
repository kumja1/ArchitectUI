using Architect.UI.Data.Binding;
using Roslyn.Generated;

namespace Architect.Build.SourceGenerator.Example;

[Bindable]
public partial class BindableModel : BindableObject
{
    [BindableProperty] private string _text;

    [BindableProperty] public partial int J { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        BindableModel model = new() { Text = "Hello, World!" };

        Console.WriteLine(model.Text);
    }
}