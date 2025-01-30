using Architect.Core;
using Architect.UI.Enums;
using Architect.Common.Interfaces;
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

    private protected Window()
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


    public override void Draw() => MainContent.Draw();

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

    internal void AddDirtyWidget(Widget widget)
    {
        if (_dirtyWidgets.Add(widget))
        {
            RenderManager.ScheduleWindowUpdate(this);
        }
    }

    public void EraseWidget(Widget widget) =>  RenderManager.ClearArea(widget.Position, widget.Size);

}
