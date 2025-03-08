using Architect.Common.Models;
using Architect.Common.Utilities;
using Architect.Common.Interfaces;
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

    public override void Arrange(IWidget parent)
    {
        base.Arrange(parent);
        foreach (Widget item in Content.Cast<Widget>())
        {
            item.OnAttachToWidget(this);

            item.Position = item.VerticalAlignment switch
            {
                VerticalAlignment.Top => item.Position with { Y = AlignmentHelper.TopCenter(item.Size, Size).Y + ItemSpacing.Height },
                VerticalAlignment.Bottom => item.Position with { Y = AlignmentHelper.BottomCenter(item.Size, Size).Y - ItemSpacing.Height },
                _ or VerticalAlignment.Stretch => AlignmentHelper.Center(item.Size, Size) with { X = 0 }
            };

            item.Position = item.HorizontalAlignment switch
            {
                HorizontalAlignment.Left => item.Position with { X = AlignmentHelper.LeftCenter(item.Size, Size).X + ItemSpacing.Width },
                HorizontalAlignment.Right => item.Position with { X = AlignmentHelper.RightCenter(item.Size, Size).X - ItemSpacing.Width },
                _ or HorizontalAlignment.Stretch => AlignmentHelper.Center(item.Size, Size) with { Y = 0 }
            };
        }
    }
}
