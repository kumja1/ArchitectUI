using System.Drawing;
using Architect.UI.Data.Binding;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace Architect.UI.Widgets.Primitives;

public class TextButton : Widget
{
    public string Text
    {
        get => GetProperty(ref field, initialValue: "Click Me")!;
        set => SetProperty(ref field, value);
    }

    public Color TextColor
    {
        get => GetProperty(ref field, initialValue: Color.Black);
        set => SetProperty(ref field, value);
    }

    public Font TextFont
    {
        get => GetProperty(ref field, initialValue: PCScreenFont.DefaultFont)!;
        set => SetProperty(ref field, value);
    }

    private readonly Button _button;
    
    public TextButton()
    {
        TextBlock textBlock = new() { TextColor = TextColor, Text = Text };

        Bind<TextButton, string>(nameof(Text))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(textBlock);

        Bind<TextButton, Color>(nameof(TextColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(textBlock);

        Bind<TextButton, Font>(nameof(TextFont))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(textBlock);

       _button = new Button
        {
            Content = textBlock
        };
    }

    public override void BeginDraw(Canvas canvas)
    {
        if (_button == null)
            return;

        Draw(canvas);
    }

    public override void Draw(Canvas canvas)
    {
        _button.Draw(canvas);
    }

    public override Size Measure(Size availableSize)
    {
        return _button.Measure(availableSize);
    }
}