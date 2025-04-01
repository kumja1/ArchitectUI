using Architect.Common.Models;
using Architect.Common.Utilities;
using Architect.UI.Widgets.Base;

namespace Architect.UI.Widgets.Layout;

public class DockPanel : MultiContentWidget
{
    public class Item : Widget;

    public Size ItemSpacing
    {
        get => GetProperty<Size>(nameof(ItemSpacing));
        set => SetProperty(nameof(ItemSpacing), value);
    }


    public override void Arrange(Size finalSize)
    {
        base.Arrange(finalSize);
        foreach (Widget item in InternalContent.Cast<Item>())
        {
            item.OnAttachToWidget(this);

            var y = item.VerticalAlignment switch
            {
                VerticalAlignment.Top => AlignmentHelper.(item.Size, finalSize).Y
                    + ItemSpacing.Height,
                VerticalAlignment.Bottom => AlignmentHelper
                    .BottomCenter(item.Size, finalSize)
                    .Y - ItemSpacing.Height,
                _ or VerticalAlignment.Stretch => AlignmentHelper
                    .CenterY(item.Size, finalSize)
                    .Y,
            };

            var x = item.HorizontalAlignment switch
            {
                HorizontalAlignment.Left => ItemSpacing.Width,
                HorizontalAlignment.Right => AlignmentHelper.CenterX(item.Size, finalSize)
                    .X - ItemSpacing.Width,
                _ or HorizontalAlignment.Stretch => AlignmentHelper
                    .Center(item.Size, finalSize)
                    .X,
            };

            item.Position = new Vector2(x, y);
        }
    }
}
