using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Architect.Common.Interfaces;
using Architect.UI.Drawing;
using Cosmos.System.Graphics;

namespace Architect.UI.Base;

public class MultiContentWidget : Widget
{
    public new ObservableCollection<IWidget> Content { get => field; set => SetProperty(ref field, value); }

    public MultiContentWidget()
    {
        Content.CollectionChanged += OnContentChanged;
    }

    private void OnContentChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var item in e.NewItems)
                Add((Widget)item);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var item in e.OldItems)
                Remove((Widget)item);
        }
        if (e.Action != NotifyCollectionChangedAction.Reset)
            MarkDirty(true);
    }

    public void Add(Widget widget)
    {
        Content.Add(widget);
        widget.OnAttachToWidget(new DrawingContext(this, widget));
    }

    public void Remove(Widget widget)
    {
        Content.Remove(widget);
        widget.Dispose();
    }

    public void Clear(bool dipose = true)
    {
        foreach (var widget in Content)
        {
            widget.Dispose();
        }
        
        Content.Clear();
        if (dipose)
            Dispose();
    }

    public override void OnDetachFromWidget() => Clear();

    public override void Draw(Canvas canvas)
    {
        foreach (var widget in Content)
        {
            widget.Draw(canvas);
        }
    }
}