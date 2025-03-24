using Architect.Common.Interfaces;
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

    public override void Arrange()
    {
        base.Arrange();

        foreach (Widget item in Content.Cast<Widget>())
        {
            item.OnAttachToWidget(this);

            var y = item.VerticalAlignment switch
            {
                VerticalAlignment.Top => AlignmentHelper.TopCenter(item.Size, Size).Y
                    + ItemSpacing.Height,
                VerticalAlignment.Bottom => AlignmentHelper.BottomCenter(item.Size, Size).Y
                    - ItemSpacing.Height,
                _ or VerticalAlignment.Stretch => AlignmentHelper.Center(item.Size, Size).Y,
            };

            var x = item.HorizontalAlignment switch
            {
                HorizontalAlignment.Left => AlignmentHelper.LeftCenter(item.Size, Size).X
                    + ItemSpacing.Width,
                HorizontalAlignment.Right => AlignmentHelper.RightCenter(item.Size, Size).X
                    - ItemSpacing.Width,
                _ or HorizontalAlignment.Stretch => AlignmentHelper.Center(item.Size, Size).X,
            };

            item.Position = new Vector2(x, y);
        }
    }
}
