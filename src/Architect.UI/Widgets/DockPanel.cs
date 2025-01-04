using System.Collections.ObjectModel;
using Architect.Common.Interfaces;
using Architect.UI.Models;
using Architect.UI.Enums;
using Architect.UI.Utils;
using Architect.Common.Models;

namespace Architect.UI.Widgets;

public class DockPanel : MultiContentWidget
{

    public class Item : Widget;

    public Size Spacing { get; set; }

    public new ObservableCollection<Item> Content { get => field; set => SetProperty(ref field, value); }

    public void Add(Item item)
    {
        Content.Add(item);
    }

    public void Remove(Item item)
    {
        Content.Remove(item);
    }

    public override void OnAttachToWidget(IDrawingContext context)
    {
        base.OnAttachToWidget(context);
        foreach (var item in Content)
        {
            var widgetContext = new DrawingContext(this, item);
            item.Position = item.VerticalAlignment switch
            {
                VerticalAlignment.Top => item.Position with { Y = AlignmentHelper.TopCenter(item.Size, Size).Y + Spacing.Height },
                VerticalAlignment.Bottom => item.Position with { Y = AlignmentHelper.BottomCenter(item.Size, Size).Y - Spacing.Height },
                VerticalAlignment.Stretch => item.Position with { Y = Context.Size.Height - item.Size.Height },
                _ => item.Position with { Y = AlignmentHelper.Center(item.Size, Size).Y }
            };

            item.Position = item.HorizontalAlignment switch
            {
                HorizontalAlignment.Left => item.Position with { X = AlignmentHelper.LeftCenter(item.Size, Size).X + Spacing.Width },
                HorizontalAlignment.Right => item.Position with { X = AlignmentHelper.RightCenter(item.Size, Size).X - Spacing.Width },
                HorizontalAlignment.Stretch => item.Position with { X = Context.Size.Width - item.Size.Width },
                _ => item.Position with { X = AlignmentHelper.Center(item.Size, Size).X }
            };
            
            item.OnAttachToWidget(widgetContext);
        }
    }


    public override void Draw()
    {
        foreach (var item in Content)
        {
            item.Draw();
        }
    }

}
