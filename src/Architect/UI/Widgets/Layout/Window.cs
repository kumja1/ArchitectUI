using Architect.UI.Widgets.Primitives;
using Architect.Utilities;
using Color = System.Drawing.Color;

namespace Architect.UI.Widgets.Layout;

public class Window : SingleContentWidget
{
    public bool IsMaximized { get; private set; }

    public Color TopBarColor
    {
        get =>
            GetProperty(
                ref field,
                initialValue: ColorHelper.GetMonoChromaticColor(BackgroundColor, 0.8f)
            );
        set => SetProperty(ref field, value);
    }

    public double TopBarWidth
    {
        get => GetProperty(ref field, initialValue: 0);
        set => SetProperty(ref field, value);
    }

    public double TopBarHeight
    {
        get => GetProperty(ref field, initialValue: 20);
        set => SetProperty(ref field, value);
    }

    protected Window()
    {
        Content =
            new DockPanel
            {
                Content =
                [
                    new InputField
                    {
                    },
                    new TextButton
                    {
                        Margin = EdgeInsets.FromSide(top: 2, right: 2),
                    }
                ]
            };
    }

    internal void OnWindowCloseInternal()
    {
        Dispose();
        OnWindowClose();
    }


    protected void OnWindowOpen()
    {
    }

    protected virtual void OnWindowClose()
    {
    }

    // public virtual void OnWindowMinimize()
    // {
    //     IsVisible = false; // Add animations later
    // }
    //
    // public virtual void OnWindowMaximize()
    // {
    //     if (!IsMaximized)
    //         IsMaximized = !IsMaximized;
    // }
    
    
}