using Cosmos.Kernel.System.Graphics;

namespace Architect.UI.Widgets.Layout;

public class DockPanel : MultiContentWidget
{
    public class Item : SingleContentWidget;

    public DockPanel()
    {
    }

    public override void BeginDraw(Canvas canvas)
    {
        throw new NotImplementedException();
    }

    public override Size Measure(Size availableSize)
    {
        double width = 0;
        double height = 0;

        Size availableSizeForChildren = availableSize - Padding.Size;
        foreach (Widget item in Content)
        {
            Size desiredSize = item.Measure(availableSizeForChildren - item.Margin.Size - new Size(width, height));

            width += desiredSize.Width + item.Margin.Width;
            height += desiredSize.Height + item.Margin.Height;
        }

        return new Size(width, height) + Padding.Size;
    } 
}