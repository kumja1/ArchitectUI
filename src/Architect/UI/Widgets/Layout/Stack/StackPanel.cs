using Architect.UI.Widgets.Layout.Alignment;

namespace Architect.UI.Widgets.Layout.Stack;

class StackPanel : MultiContentWidget
{
    public virtual StackOrientation Orientation
    {
        get => GetProperty(ref field, initialValue: StackOrientation.Horizontal);
        set => SetProperty(ref field, value);
    }

    static StackPanel()
    {
        InitializePropertyTable(typeof(StackPanel));
    }

    protected StackPanel()
    {
        VerticalAlignment = VerticalAlignment.Stretch;
        HorizontalAlignment = HorizontalAlignment.Stretch;
    }

    public override Size Measure(Size availableSize)
    {
        Size paddedSize = new(
            Math.Max(0, availableSize.Width - Padding.Width),
            Math.Max(0, availableSize.Height - Padding.Height)
        );

        Size measuredSize = Orientation switch
        {
            StackOrientation.Horizontal => MeasureHorizontal(paddedSize),
            StackOrientation.Vertical => MeasureVertical(paddedSize),
            _ => throw new InvalidOperationException(),
        };

        return measuredSize + Padding.Size;
    }

    private Size MeasureHorizontal(Size availableSize)
    {
        double width = 0;
        double height = 0;

        foreach (Widget item in Content)
        {
            ArgumentNullException.ThrowIfNull(item);

            item.Measure(
                new Size(
                    Math.Max(0, availableSize.Width - width - item.Margin.Width),
                    Math.Max(0, availableSize.Height - item.Margin.Height)
                )
            );

            width += item._mesureSize.Width + item.Margin.Width;
            height = Math.Max(
                height,
                item._mesureSize.Height + item.Margin.Top + item.Margin.Bottom
            );
        }

        return new Size(width, height);
    }

    private Size MeasureVertical(Size availableSize)
    {
        double width = 0;
        double height = 0;

        foreach (Widget item in Content)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.Measure(
                new Size(
                    Math.Max(0, availableSize.Width - item.Margin.Width),
                    Math.Max(0, availableSize.Height - height - item.Margin.Height)
                )
            );

            width = Math.Max(width, item._mesureSize.Width + item.Margin.Width);
            height += item._mesureSize.Height + item.Margin.Height;
        }

        return new Size(width, height);
    }

    protected override void ArrangeContent()
    {
        if (Orientation == StackOrientation.Horizontal)
            ArrangeHorizontal();
        else
            ArrangeVertical();
    }

    private void ArrangeHorizontal()
    {
        double currentX = X + Padding.Left;
        foreach (Widget item in Content)
        {
            item.ArrangeInternal(
                new Rect(
                    currentX + item.Margin.Left,
                    Y + Padding.Top + item.Margin.Top,
                    item._mesureSize.Width,
                    item._mesureSize.Height
                )
            );
            currentX += item._mesureSize.Width + item.Margin.Width;
        }
    }

    private void ArrangeVertical()
    {
        double currentY = Y + Padding.Top;
        foreach (Widget item in Content)
        {
            item.ArrangeInternal(
                new Rect(
                    X + Padding.Left + item.Margin.Left,
                    currentY + item.Margin.Top,
                    item._mesureSize.Width,
                    item._mesureSize.Height
                )
            );
            currentY += item._mesureSize.Height + item.Margin.Height;
        }
    }
}