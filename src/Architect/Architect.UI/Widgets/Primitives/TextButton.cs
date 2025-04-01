using System.Drawing;
using Architect.UI.Data.Core;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Size = Architect.Common.Models.Size;

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

    public Font TextFont
    {
        get => GetProperty(nameof(TextFont), defaultValue: PCScreenFont.Default)!;
        set => SetProperty(nameof(TextFont), value);
    }

    public TextButton()
    {
        Content = new TextBlock { TextColor = TextColor, Text = Text }.GetReference(
            out TextBlock textBlock
        );

        Bind<TextButton, string>(nameof(Text))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(textBlock);

        Bind<TextButton, Color>(nameof(TextColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(textBlock);

        Bind<TextButton, Font>(nameof(TextFont))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(textBlock, nameof(TextBlock.Font));
    }
}
