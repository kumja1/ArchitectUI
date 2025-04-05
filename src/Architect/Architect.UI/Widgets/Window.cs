using System.Drawing;
using Architect.Common.Utilities;
using Architect.UI.Widgets.Base;
using Cosmos.System.Graphics;

namespace Architect.UI.Widgets;

public class Window : Widget
{
    public bool IsMaximized { get; private set; } = false;

    public Color TopBarColor
    {
        get =>
            GetProperty(
                nameof(TopBarColor),
                defaultValue: ColorHelper.GetMonoChromaticColor(BackgroundColor, 0.8f)
            );
        set => SetProperty(nameof(TopBarColor), value);
    }

    public double TopBarWidth
    {
        get => GetProperty(nameof(TopBarWidth), defaultValue: 0);
        set => SetProperty(nameof(TopBarWidth), value);
    }
    public double TopBarHeight
    {
        get => GetProperty(nameof(TopBarHeight), defaultValue: 20);
        set => SetProperty(nameof(TopBarHeight), value);
    }

    protected Window()
        : base() { }

    public override void Draw(Canvas canvas)
    {
        DrawBackground(canvas);
        InternalContent.Draw(canvas);
    }

    public virtual void OnWindowClose() => Dispose();

    public virtual void OnWindowMinimize()
    {
        IsVisible = false; // Add animations later
    }

    public virtual void OnWindowMaximize()
    {
        if (!IsMaximized)
            IsMaximized = !IsMaximized;
    }

    public override void Dispose()
    {
        InternalContent.Dispose();
        base.Dispose();
    }
}
