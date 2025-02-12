using Architect.Common.Models;
using Architect.UI.Enums;
using Architect.Common.Interfaces;
using Architect.Common.Utils;
using Architect.UI.Drawing;
using Architect.UI.Base;

namespace Architect.UI.Layout;

class Stack : MultiContentWidget
{
    public int Spacing { get; set; }

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
}
