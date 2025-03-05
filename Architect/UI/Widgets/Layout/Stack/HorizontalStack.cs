using Architect.Common.Interfaces;
using Architect.Common.Utilities;

namespace Architect.UI.Layout;

class HorizontalStack : Stack
{
    public override void Arrange(IWidget parent)
    {
        var currentX = Position.X;
        foreach (var widget in Content)
        {
            widget.OnAttachToWidget(this);
            widget.Position = VerticalAlignment switch
            {
                VerticalAlignment.Center => widget.Position with { Y = Position.Y + AlignmentHelper.Center(Size, widget.Size).Y, X = currentX },
                VerticalAlignment.Bottom => widget.Position with { Y = Position.Y + AlignmentHelper.Bottom(Size, widget.Size).Y, X = currentX },
                _ => widget.Position with { X = currentX }
            };
            currentX += widget.Size.Width + Spacing;
        }
    }
}
