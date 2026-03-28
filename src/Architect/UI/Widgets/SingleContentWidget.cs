using Cosmos.Kernel.System.Graphics;

namespace Architect.UI.Widgets;

public class SingleContentWidget : Widget
{
    private Widget? _content;

    public Widget? Content
    {
        get => GetProperty(ref _content);
        set => SetProperty(ref _content, value, propertyChanged: OnContentChanged);
    }

    private void OnContentChanged(string name, object? previousValue, object value)
    {
        if (previousValue is not Widget currentWidget || value is not Widget newWidget)
            return;

        currentWidget.Dispose();
        newWidget.OnAttachToWidgetInternal(this);
    }

    public override void BeginDraw(Canvas canvas)
    {
        if (_content == null)
            return;

        DrawBackground(canvas);
        Draw(canvas);
    }

    public override void Draw(Canvas canvas) => 
        _content?.Draw(canvas);

    protected override void Arrange(Rect bounds)
    {
        Rect inner = new(
            bounds.X + Padding.Left,
            bounds.Y + Padding.Top,
            Math.Max(0, bounds.Width - Padding.Left - Padding.Right),
            Math.Max(0, bounds.Height - Padding.Top - Padding.Bottom)
        );

        Rect childRect = new(
            inner.X + _content.Margin.Left,
            inner.Y + _content.Margin.Top,
            Math.Max(0, inner.Width - _content.Margin.Left - _content.Margin.Right),
            Math.Max(0, inner.Height - _content.Margin.Top - _content.Margin.Bottom)
        );

        _content.ArrangeInternal(childRect);
    }

    public override Size Measure(Size availableSize)
    {
        if (_content == null)
            return Size.Zero;

        double remainingWidth = Math.Max(0, availableSize.Width - Padding.Width - _content.Margin.Width);
        double remainingHeight = Math.Max(0, availableSize.Height - Padding.Height - _content.Margin.Height);
        Size contentSize = _content.Measure(new Size(remainingWidth, remainingHeight));

        DesiredSize = contentSize + _content.Margin.Size + Padding.Size;
        return DesiredSize;
    }

    public override void Dispose()
    {
        _content?.Dispose();
        base.Dispose();
    }
}