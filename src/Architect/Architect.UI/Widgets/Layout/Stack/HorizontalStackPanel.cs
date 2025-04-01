using Architect.Common.Models;
using Architect.Common.Utilities;

namespace Architect.UI.Widgets.Layout.Stack;

class HorizontalStackPanel : StackPanel
{
    public override void Arrange(Rect finalRect)
    {
        var currentX = Position.X;
        foreach (var widget in InternalContent)
        {
            widget.OnAttachToWidget(this);

            currentX += widget.Size.Width + Spacing;
        }
    }
}
