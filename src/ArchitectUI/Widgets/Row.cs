using ArchitectUI.Utils;
using Architect.Enums;

namespace Architect.Widgets;

class Row : Widget
{
    public int Spacing { get; set; }

    public new List<Widget> Content { get; set; }



    public void Add(Widget widget) => Content.Add(widget);

    public void Remove(Widget widget) => Content.Remove(widget);


    public override void OnAttachToWidget(DrawingContext context)
    {
        base.OnAttachToWidget(context);
        // Attach all the widgets to the context
        foreach (var widget in Content)
        {
            var widgetContext = new DrawingContext(this, widget);
            // Calculate postions based on the alignment
            if (widget.VerticalAlignment == VerticalAlignment.Center)
            {
                widget.Position = widget.Position with { Y = Position.Y + AlignmentHelper.Center(Size, widget.Size).Y };
            }
            else if (widget.VerticalAlignment == VerticalAlignment.Bottom)
            {
                widget.Position = widget.Position with { Y = Position.Y + AlignmentHelper.Bottom(Size, widget.Size).Y };
            }
            widget.OnAttachToWidget(widgetContext);
        }
    }


    public override void OnDetachFromWidget()
    {
        foreach (var widget in Content)
        {
            widget.OnDetachFromWidget();
            // widget.Dispose(); No need to call the widget dispose method since the context will be disposed
        }
    }

    public override void Draw()
    {
        var currentX = Position.X;
        for (var i = 0; i < Content.Count; i++)
        {
            var widget = Content[i];
            widget.Position = Position with { X = currentX };
            widget.Draw();
            currentX += widget.Size.Width + Spacing;
        }
    }


}
