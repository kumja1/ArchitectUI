using System.Drawing;
using Architect.Common.Utilities;
using Architect.Core.Rendering;
using Architect.UI.Widgets.Base;
using Architect.UI.Data.Core;
using Architect.UI.Widgets.Layout;
using Architect.UI.Widgets.Primitives;
using Cosmos.System.Graphics;
using Size = Architect.Common.Models.Size;
using Architect.UI.Widgets.Layout.Stack;

namespace Architect.UI.Widgets;

public class Window : Widget
{
    public bool IsMaximized;

    public Color TopBarColor
    {
        get =>
            GetProperty(
                nameof(TopBarColor),
                defaultValue: ColorHelper.GetMonoChromaticColor(BackgroundColor, 0.8f)
            );
        set => SetProperty(nameof(TopBarColor), value);
    }

    public Size TopBarSize
    {
        get => GetProperty(nameof(TopBarSize), defaultValue: new Size(0, 20));
        set => SetProperty(nameof(TopBarSize), value);
    }

    protected Window()
        : base()
    {
        InternalContent = new DockPanel
        {
            Content =
            [
                new DockPanel.Item
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Size = TopBarSize,
                    BackgroundColor = TopBarColor,
                    Content = new StackPanel
                    {
                        Content =
                        [
                            new ImageButton
                            {
                                Size = new Size(100, 30),
                            }.GetReference(out TextButton maximizeButton),
                            new TextButton
                            {
                                Text = "Close",
                                Size = new Size(100, 30),
                            }.GetReference(out TextButton closeButton),
                        ],
                    },
                }.GetReference(out DockPanel.Item topBar),
                new DockPanel.Item
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Content = Content,
                },
            ],
        };

        maximizeButton.Clicked += (_, _) => OnWindowMaximize();
        closeButton.Clicked += (_, _) => OnWindowClose();

        Bind<Window, Color>(nameof(TopBarColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(topBar, nameof(BackgroundColor));
    }

    public override Size Measure(Size availableSize = default)
    {
        availableSize = Size.Clamp(
            availableSize,
            Size.Zero,
            RenderManager.Instance.CanvasSize
        );
        return base.Measure(availableSize);
    }

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
