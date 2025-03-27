using Architect.Common.Interfaces;
using Architect.Common.Models;
using Cosmos.System.Graphics;

namespace Architect.UI.Widgets.Base;

public class MultiContentWidget : Widget
{
    protected new List<IWidget> InternalContent;

    /// <summary>
    /// Gets or sets the collection of content widgets.
    /// </summary>
    public new List<IWidget> Content
    {
        get => GetProperty<List<IWidget>>(nameof(Content), defaultValue: []);
        set => SetProperty(nameof(Content), value);
    }

    public void AddChild(Widget widget)
    {
        Content?.Add(widget);
        widget.OnAttachToWidget(this);
    }

    public void RemoveChild(IWidget widget)
    {
        Content?.Remove(widget);
        widget.Dispose();
    }

    public override void Draw(Canvas canvas)
    {
        DrawBackground(canvas);
        foreach (var widget in InternalContent)
        {
            widget.Draw(canvas);
        }
    }

    public override void Dispose()
    {
        ArgumentNullException.ThrowIfNull(Content, nameof(Content));
        Content.RemoveAll(x =>
        {
            x.Dispose();
            return true;
        });

        GC.SuppressFinalize(this);
    }

    public override Size Measure(Size availableSize)
    {
        var width = 0;
        var height = 0;

        foreach (Widget item in Content.Cast<Widget>())
        {
            var desiredSize = item.Measure(availableSize);

            width = Math.Max(width, desiredSize.Width);
            height = Math.Max(height, desiredSize.Height);
        }

        return new Size(width, height);
    }

    public override Size GetNaturalSize() =>
        Size
        + InternalContent.Aggregate(
            Size.Zero,
            (current, widget) =>
            {
                var desiredSize = widget.GetNaturalSize();
                return new Size(
                    Math.Max(current.Width, desiredSize.Width),
                    Math.Max(current.Height, desiredSize.Height)
                );
            }
        );
}
