using Architect.Common.Interfaces;
using Architect.UI.Enums;
using Architect.UI.Models;
using Architect.UI.Utils;

namespace Architect.UI.Widgets;

class Stack : Widget
{
    public int Spacing { get; set; }

    private List<IWidget> field = new List<IWidget>();
    public new List<IWidget> Content { get => field; set => SetProperty(ref field, value); }

    public void Add(Widget widget)
    {
        var context = new DrawingContext(this, widget);  
        widget.Context = context;
        widget.OnAttachToWidget(context);
    }

    public void Remove(Widget widget)
    {
        Content.Remove(widget);
        widget.OnDetachFromWidget();
    }

    public void Clear()
    {
        foreach (var widget in Content)
            Remove((Widget)widget);
    }
    


    public override void OnAttachToWidget(IDrawingContext context)
    {
        base.OnAttachToWidget(context);

        // Attach all the widgets to the context
        var currentX = Position.X;
        foreach (var widget in Content)
        {
            var widgetContext = new DrawingContext(this, widget);
            widget.Position = VerticalAlignment switch
            {
                VerticalAlignment.Center => widget.Position with { Y = Position.Y + AlignmentHelper.Center(widgetContext.Size, widget.Size).Y },
                VerticalAlignment.Bottom => widget.Position with { Y = Position.Y + AlignmentHelper.Bottom(widgetContext.Size, widget.Size).Y },
                _ => widget.Position with { X = currentX }
            };

            widget.OnAttachToWidget(widgetContext);
            currentX += widget.Size.Width + Spacing;
        }
    }



    public override void OnDetachFromWidget() => Clear();


    public override void Draw()
    {
        foreach (var widget in Content)
        {
            widget.Draw();
        }
    }


}
