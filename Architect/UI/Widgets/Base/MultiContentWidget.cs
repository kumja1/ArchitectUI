using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Architect.Common.Interfaces;
using Architect.Common.Models;



namespace Architect.UI;

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
            {
                Add((Widget)item);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var item in e.OldItems)
            {
                Remove((Widget)item);
            }
        }
        if (e.Action != NotifyCollectionChangedAction.Reset)
            MarkDirty(true);
    }

    public void Add(Widget widget)
    {
        var context = new DrawingContext(this, widget);
        widget.Context = context;
        widget.OnAttachToWidget(context);
    }

    public void Remove(Widget widget)
    {
        Content.Remove(widget);
        widget.Dispose();
    }

    public void Clear()
    {
        foreach (var widget in Content)
        {
            widget.Dispose();
        }
        Content.Clear();
    }

    public override void OnDetachFromWidget() => Clear();

    public override void Draw()
    {
        foreach (var widget in Content)
        {
            widget.Draw();
        }
    }
}