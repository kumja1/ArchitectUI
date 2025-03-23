using System.Drawing;
using Architect.UI.Widgets.Bindings;

namespace Architect.UI.Widgets.Primitives;

public class TextButton : Button
{
    public string Text
    {
        get => GetProperty(nameof(Text), defaultValue: "Click Me!")!;
        set => SetProperty(nameof(Text), value);
    }

    public Color TextColor
    {
        get => GetProperty(nameof(TextColor), defaultValue: Color.Black);
        set => SetProperty(nameof(TextColor), value);
    }

    public TextButton()
    {
        Content = new TextBlock { TextColor = TextColor, Text = Text }.GetReference(
            out TextBlock textBlock
        );

        Bind<TextBlock, string>(nameof(Text))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(textBlock);

        Bind<TextBlock, Color>(nameof(TextColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(textBlock);
    }
}
