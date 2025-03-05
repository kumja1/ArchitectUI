using Architect.Common.Models;
using Architect.Common.Utilities;
using Architect.Common.Interfaces;
using Cosmos.System.Graphics;
using Architect.UI.Base;

namespace Architect.UI.Layout;

public class DockPanel : MultiContentWidget
{
    public class Item : Widget;

    public Size Spacing { get => field; set => SetProperty(ref field, value); }

    public override void Arrange(IWidget parent)
    {
        base.Arrange(parent);
        foreach (Widget item in Content.Cast<Widget>())
        {
            item.OnAttachToWidget(this);
            
            item.Position = item.VerticalAlignment switch
            {
                VerticalAlignment.Top => item.Position with { Y = AlignmentHelper.TopCenter(item.Size, Size).Y + Spacing.Height },
                VerticalAlignment.Bottom => item.Position with { Y = AlignmentHelper.BottomCenter(item.Size, Size).Y - Spacing.Height },
                _ or VerticalAlignment.Stretch => AlignmentHelper.Center(item.Size, Size) with { X = 0 }
            };

            item.Position = item.HorizontalAlignment switch
            {
                HorizontalAlignment.Left => item.Position with { X = AlignmentHelper.LeftCenter(item.Size, Size).X + Spacing.Width },
                HorizontalAlignment.Right => item.Position with { X = AlignmentHelper.RightCenter(item.Size, Size).X - Spacing.Width },
                _ or HorizontalAlignment.Stretch => AlignmentHelper.Center(item.Size, Size) with { Y = 0 }
            };
        }
    }

    public override void Draw(Canvas canvas)
    {

        foreach (var item in Content)
        {
            item.Draw(canvas);
        }
    }

}
