using System.Drawing;
using Architect.Common.Utilities;
using Architect.UI.Widgets.Base;
using Architect.UI.Widgets.Bindings;
using Architect.UI.Widgets.Layout;
using Architect.UI.Widgets.Primitives;
using Cosmos.System.Graphics;
using Size = Architect.Common.Models.Size;

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
        get => GetProperty(nameof(CurrentSize), defaultValue: Size.Zero);
        private set => SetProperty(nameof(CurrentSize), value);
    }

    public bool IsMaximized
    {
        get => GetProperty<bool>(nameof(IsMaximized));
        private set => SetProperty(nameof(IsMaximized), value);
    }

    public Color TopBarColor
    {
        get =>
            GetProperty(
                nameof(TopBarColor),
                defaultValue: ColorHelper.GetMonoChromaticColor(BackgroundColor, 0.8f)
            );
        set => SetProperty(nameof(TopBarColor), value);
    }

    private readonly DockPanel InternalContent;

    protected Window()
    {
        MinSize = new Size(100, 100);
        MaxSize = new Size(800, 600);

        InternalContent = new DockPanel
        {
            Content =
            [
                new DockPanel.Item
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Size = new Size(0, 30),
                    BackgroundColor = TopBarColor,

                    Content = new Stack
                    {
                        Content =
                        [
                            new TextButton
                            {
                                Text = "Maximize",
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

        Bind<DockPanel.Item, Color>(nameof(TopBarColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To(topBar, nameof(BackgroundColor));
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

    public virtual void OnWindowMaximize() { }

    public override void Dispose()
    {
        base.Dispose();
        InternalContent.Dispose();
    }
}

