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

    public override Size Measure(Size availableSize = default)
    {
        double width = 0;
        double height = 0;

        foreach (Widget item in Content.Cast<Widget>())
        {
            var desiredSize = item.Measure(availableSize);
            width = Math.Max(width, desiredSize.Width);
            height = Math.Max(height, desiredSize.Height);
        }

        return new Size(width, height);
    }

    protected override void ArrangeContent()
    {
        foreach (var item in InternalContent)
        {
            item.Measure(
                new Size(
                    Math.Max(0, SizeX - item.Margin.Width),
                    Math.Max(0, SizeY - item.Margin.Height)
                )
            );
            var rect = new Rect(
                X + item.Margin.Left,
                Y + item.Margin.Top,
                item.MeasuredSize.Width,
                item.MeasuredSize.Height
            );
            item.Arrange(rect);
        }
    }

    protected override void AttachContent(ref object currentValue, object? widget)
    {
        if (currentValue is List<IWidget> currentWidgets && widget is List<IWidget> newWidgets)
        {
            currentWidgets.RemoveAll(x =>
            {
                x.Dispose();
                return true;
            });

            foreach (var item in newWidgets)
            {
                item.OnAttachToWidget(this);
            }
            InternalContent = newWidgets;
        }
    }

    public override Size GetNaturalSize() =>
        Padding.Size
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
