using Size = Architect.Common.Models.Size;
using Architect.UI.Drawing;
using Cosmos.System.Graphics;
using Architect.UI.Base;
using Architect.UI.Layout;
using Architect.UI.Primitives;
using System.Drawing;

namespace Architect.UI;

public class Window : Widget
{
    public Size MaxSize { get; init; }
    public Size MinSize { get; init; }
    public Size CurrentSize { get; private set; }
    private DockPanel ContentCore { get => field; set => SetProperty(ref field, value); }


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
