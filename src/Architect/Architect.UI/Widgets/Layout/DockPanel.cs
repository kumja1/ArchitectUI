using Architect.Common.Models;
using Architect.Common.Utilities;
using Architect.UI.Widgets.Base;
using Architect.UI.Widgets.Primitives;

namespace Architect.UI.Widgets.Layout;

public class DockPanel : MultiContentWidget
{
    public class Item : Widget;

    public DockPanel()
    {
    
    }

    public override Size Measure(Size availableSize)
    {
        double width = 0;
        double height = 0;

        var paddedSize = new Size(
            availableSize.Width - Padding.Width,
            availableSize.Height - Padding.Height
        );

        foreach (var item in Content)
        {
            if (item.Content != null)
            {
                item.Content.Measure(
                    new Size(
                        Math.Max(0,paddedSize.Width - item.Content.Margin.Width),
                        Math.Max(0,paddedSize.Height - height - item.Margin.Height)
                    )
                );

                width = Math.Max(width, item.MeasuredSize.Width + item.Margin.Width);
                height += item.MeasuredSize.Height + item.Margin.Height;
            }
        }

        return new Size(width, height) + Padding.Size;
    }
}
