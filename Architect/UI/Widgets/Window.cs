using Size = Architect.Common.Models.Size;
using Architect.UI.Drawing;
using Cosmos.System.Graphics;
using Architect.UI.Base;
using Architect.UI.Layout;
using Architect.UI.Primitives;

namespace Architect.UI;

public class Window : Widget
{
    public Size MaxSize { get; init; }
    public Size MinSize { get; init; }
    public Size CurrentSize { get; private set; }
    private DockPanel MainContent { get => field; set => SetProperty(ref field, value); }


     protected Window()
    {
        MinSize = new Size(100, 100);
        MaxSize = new Size(800, 600);
        Context = new DrawingContext(this, Content);

        Button? closeButton = new()
        {
            Text = "Close",
            Size = new Size(100, 30),
        };

        Button maximizeButton = new()
        {
            Text = "Maximize",
        };

        closeButton.Clicked += (_, _) => OnWindowClose();
        maximizeButton.Clicked += (_, _) => OnWindowMaximize();

        MainContent = new DockPanel
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


    public override void Draw(Canvas canvas) => MainContent.Draw(canvas);

    public virtual void OnWindowClose() => Dispose();

    public virtual void OnWindowMinimize()
    {
        IsVisible = false; // Add animations later
    }

    public virtual void OnWindowMaximize() { }


    public override void Dispose()
    {
        MainContent.Dispose();
        Context.Dispose();
    }

}
