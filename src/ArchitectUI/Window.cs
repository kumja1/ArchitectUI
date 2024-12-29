using Architect.Widgets;

namespace Architect;


class Window : Widget
{

    public Size MaxSize { get; init; }

    public Size MinSize { get; init; }

    public Window()
    {
        MinSize = new Size(100, 100);
        MaxSize = new Size(800, 600);
    }

    

    public Size CurrentSize
    {
        get => field;
        set
        {
            if (value.Width < MinSize.Width || value.Height < MinSize.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Size cannot be smaller than MinSize");
            }
            if (value.Width > MaxSize.Width || value.Height > MaxSize.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Size cannot be bigger than MaxSize");
            }
            field = value;
        }
    }


    public override void Draw()
    {
        throw new NotImplementedException();
    }
}
