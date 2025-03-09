using Size = Architect.Common.Models.Size;
using Cosmos.System.Graphics;
using Architect.UI.Widgets.Base;
using Architect.UI.Widgets.Layout;
using Architect.UI.Widgets.Primitives;

namespace Architect.UI.Widgets;

public class Window : Widget
{
    public Size MaxSize
    {
        get => GetProperty<Size>(nameof(MaxSize));
        init => SetProperty(nameof(MaxSize), value);
    }

    public Size MinSize
    {
        get => GetProperty<Size>(nameof(MinSize));
        init => SetProperty(nameof(MinSize), value);
    }

    public Size CurrentSize
    {
        get => GetProperty<Size>(nameof(CurrentSize));
        private set => SetProperty(nameof(CurrentSize), value);
    }

    private DockPanel ContentCore
    {
        get => GetProperty<DockPanel>(nameof(ContentCore));
        set => SetProperty(nameof(ContentCore), value);
    }

    protected Window()
    {
        MinSize = new Size(100, 100);
        MaxSize = new Size(800, 600);

        TextButton? closeButton = new()
        {
            Text = "Close",
            Size = new Size(100, 30),
        };

        TextButton maximizeButton = new()
        {
            Text = "Maximize",
        };

        closeButton.Clicked += (_, _) => OnWindowClose();
        maximizeButton.Clicked += (_, _) => OnWindowMaximize();

        ContentCore = new DockPanel
        {
            Content = [
                new DockPanel.Item {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Size = new Size(0, 30),
                    Content = new Stack {
                        Content = [
                            maximizeButton,
                            closeButton
                        ]
                    }
                },

                new DockPanel.Item {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Content = Content
                }
            ]
        };
    }

    public override void Draw(Canvas canvas) => ContentCore.Draw(canvas);

    public virtual void OnWindowClose() => Dispose();

    public virtual void OnWindowMinimize()
    {
        IsVisible = false; // Add animations later
    }

    public virtual void OnWindowMaximize() { }

    public override void Dispose()
    {
        base.Dispose();
        ContentCore.Dispose();
    }
}