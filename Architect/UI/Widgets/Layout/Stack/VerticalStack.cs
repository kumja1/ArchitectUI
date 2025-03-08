using Architect.Common.Interfaces;
using Architect.Common.Utilities;

namespace Architect.UI.Widgets.Layout;

class VerticalStack : Stack
{
    public override void Arrange(IWidget parent)
    {
        var currentY = Position.Y;
        foreach (var widget in Content)
        {
            widget.OnAttachToWidget(this);

            widget.Position = HorizontalAlignment switch
            {
                HorizontalAlignment.Center => widget.Position with { X = Position.X + AlignmentHelper.Center(Size, widget.Size).X, Y = currentY },
                HorizontalAlignment.Right => widget.Position with { X = Position.X + AlignmentHelper.Right(Size, widget.Size).X, Y = currentY },
                _ => widget.Position with { Y = currentY }
            };

            currentY += widget.Size.Height + Spacing;
        }
    }
}
