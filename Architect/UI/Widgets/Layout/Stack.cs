using Architect.Common.Interfaces;
using Architect.Common.Utils;
using Architect.UI.Base;

namespace Architect.UI.Layout;

class Stack : MultiContentWidget
{
    public int Spacing { get; set; }

    public override void OnAttachToWidget(IWidget parent)
    {
        base.OnAttachToWidget(parent);
        
        // Attach all the widgets to the context
        var currentX = Position.X;
        foreach (var widget in Content)
        {
            widget.Position = VerticalAlignment switch
            {
                VerticalAlignment.Center => widget.Position with { Y = Position.Y + AlignmentHelper.Center(Size, widget.Size).Y },
                VerticalAlignment.Bottom => widget.Position with { Y = Position.Y + AlignmentHelper.Bottom(Size, widget.Size).Y },
                _ => widget.Position with { X = currentX }
            };

            widget.OnAttachToWidget(this);
            currentX += widget.Size.Width + Spacing;
        }
    }
}
