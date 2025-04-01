using Size = Architect.Common.Models.Size;
using Architect.Common.Utilities;
using Architect.Common.Models;

namespace Architect.UI.Widgets.Layout.Stack;

class VerticalStackPanel : StackPanel
{
    public override void Arrange(Rect finalRect)
    {
        var currentY = Position.Y;
        foreach (var widget in InternalContent)
        {
            widget.OnAttachToWidget(this);

            currentY += widget.Size.Height + Spacing;
        }
    }
}
