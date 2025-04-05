using Architect.Common.Interfaces;
using Architect.Common.Models;
using Cosmos.System.Graphics;

namespace Architect.UI.Widgets.Base;

public abstract class MultiContentWidget : Widget, IMultiContentWidget
{
    /// <summary>
    /// Gets or sets the collection of content widgets.
    /// </summary>
    public new List<IWidget> Content
    {
        get => GetProperty<List<IWidget>>(nameof(Content), defaultValue: []);
        set => SetProperty(nameof(Content), value);
    }

    public virtual void AddContent(Widget widget)
    {
        Content?.Add(widget);

        widget.OnAttachToWidget(this);
        ArrangeContent();
    }

    public virtual void RemoveContent(IWidget widget)
    {
        Content?.Remove(widget);
        widget.Dispose();
    }

    public override void Draw(Canvas canvas)
    {
        DrawBackground(canvas);
        foreach (var widget in Content)
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

    public abstract override Size Measure(Size availableSize = default);
    protected override void ArrangeContent()
    {
        foreach (var widget in Content)
        {
            widget.Arrange(
                new Rect(
                    X + Padding.Left + widget.Margin.Left,
                    Y + Padding.Top + widget.Margin.Top,
                    widget.MeasuredSize
                )
            );
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

            Content = newWidgets;
        }
    }

    public override Size GetNaturalSize() =>
        Padding.Size
        + Content.Aggregate(
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
