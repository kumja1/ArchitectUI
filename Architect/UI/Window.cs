using Architect.Core.Interfaces;
using Architect.UI.Enums;
using Architect.UI.Interfaces;
using Architect.UI.Models;
using Size = Architect.Common.Models.Size;

namespace Architect.UI;


public class Window : Widget
{
    public Size MaxSize { get; init; }
    public Size MinSize { get; init; }
    public Size CurrentSize { get; private set; }
    private DockPanel MainContent { get => field; set => SetProperty(ref field, value); }

    private readonly HashSet<IWidget> _dirtyWidgets = [];
   
    public Window()
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



    private void OnWindowMaximize(object? sender, IEvent e)
    {
        throw new NotImplementedException();
    }


    public override void Draw() => MainContent.Draw();

    public virtual void OnWindowClose() => Erase();

    public virtual void OnWindowMinimize()
    {
        IsVisible = false; // Add animations later
    }

    public virtual void OnWindowMaximize() { }


    public override void Dispose()
    {
        MainContent.Dispose();
        OnWindowClose();
        Context.Dispose();
    }
}
