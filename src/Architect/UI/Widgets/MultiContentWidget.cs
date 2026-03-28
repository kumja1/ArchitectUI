using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Cosmos.Kernel.System.Graphics;

namespace Architect.UI.Widgets;

public class MultiContentWidget : Widget
{
    public ObservableCollection<Widget> Content => GetProperty(ref field, initialValue: []);

    protected MultiContentWidget()
    {
        Content!.CollectionChanged += OnContentChanged;
    }

    // public virtual void AddContent(Widget widget)
    // {
    //     AttachContent(widget);
    //     ArrangeContent();
    // }
    //
    // public virtual void RemoveContent(Widget widget)
    // {
    //     Content.Remove(widget);
    //     widget.Dispose();
    // }

    private void OnContentChanged(object? _, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is { Count: > 0 })
            foreach (Widget widget in e.OldItems)
                widget.Dispose();

        if (e.NewItems == null)
            return;

        foreach (Widget widget in e.NewItems)
            widget.OnAttachToWidgetInternal(this);
    }

    public override void BeginDraw(Canvas canvas)
    {
        if (Content.Count == 0)
            return;

        DrawBackground(canvas);
        Draw(canvas);
    }

    public override void Draw(Canvas canvas)
    {
        foreach (Widget widget in Content)
            widget.BeginDraw(canvas);
    }

    public override void Dispose()
    {
        if (Content is { Count: > 0 })
            Content.Clear();

        base.Dispose();
    }

    protected override void Arrange(Rect bounds)
    {
        foreach (Widget widget in Content)
        {
            Rect inner = new(
                bounds.X + Padding.Left,
                bounds.Y + Padding.Top,
                Math.Max(0, bounds.Width - Padding.Left - Padding.Right),
                Math.Max(0, bounds.Height - Padding.Top - Padding.Bottom)
            );

            Rect childRect = new(
                inner.X + widget.Margin.Left,
                inner.Y + widget.Margin.Top,
                Math.Max(0, inner.Width - widget.Margin.Left - widget.Margin.Right),
                Math.Max(0, inner.Height - widget.Margin.Top - widget.Margin.Bottom)
            );

            widget.ArrangeInternal(childRect);
        }
    }

    public override Size Measure(Size availableSize)
    {
        throw new NotImplementedException();
    }
}