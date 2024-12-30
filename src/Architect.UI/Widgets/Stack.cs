using Architect.UI.Enums;
using Architect.UI.Models;
using Architect.UI.Utils;

namespace Architect.UI;

class Stack : Widget
{
    public int Spacing { get; set; }

    public new List<Widget> Content { get => field; set => SetProperty(ref field, value); }

    public void Add(Widget widget) => Content.Add(widget);

    public void Remove(Widget widget) => Content.Remove(widget);


    public override void OnAttachToWidget(DrawingContext context)
    {
        base.OnAttachToWidget(context);
        // Attach all the widgets to the context
        var currentX = Position.X;
        foreach (var widget in Content)
        {
            var widgetContext = new DrawingContext(this, widget);

            // Calculate postions based on the alignment
            if (widget.VerticalAlignment == VerticalAlignment.Center)
            {
                widget.Position = widget.Position with { Y = Position.Y + AlignmentHelper.Center(widgetContext.Size, widget.Size).Y };
            }
            else if (widget.VerticalAlignment == VerticalAlignment.Bottom)
            {
                widget.Position = widget.Position with { Y = Position.Y + AlignmentHelper.Bottom(widgetContext.Size, widget.Size).Y };
            }
            widget.Position = Position with { X = currentX };

            widget.OnAttachToWidget(widgetContext);
            currentX += widget.Size.Width + Spacing;

        }
    }


    public override void OnDetachFromWidget()
    {
        foreach (var widget in Content)
        {
            widget.OnDetachFromWidget();
            // widget.Dispose(); No need to call the widget dispose method since the context will be disposed
        }
        Content.Clear();
    }


    public override void Draw()
    {
        foreach (var widget in Content)
        {
            widget.Draw();
        }
    }


}
