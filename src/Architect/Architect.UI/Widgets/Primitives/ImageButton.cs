using Cosmos.System.Graphics;
using Size = Architect.Common.Models.Size;

namespace Architect.UI.Widgets.Primitives;

public class ImageButton : Button
{
    public Image? Image
    {
        get => GetProperty<Image?>(nameof(Image));
        set => SetProperty(nameof(Image), value);
    }

    public Size ImageSize
    {
        get => GetProperty(nameof(ImageSize), defaultValue: Size.Zero);
        set => SetProperty(nameof(ImageSize), value);
    }

    public override void Draw(Canvas canvas)
    {
        if (Image != null)
            canvas.DrawImage(Image, Position.X, Position.Y, ImageSize.Width, ImageSize.Height);

        base.Draw(canvas);
    }

    public override Size GetNaturalSize() => base.GetNaturalSize() + ImageSize;
}
