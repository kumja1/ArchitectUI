using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Architect.Common.Interfaces;
using Architect.UI.Enums;
using Architect.UI.Models;
using Architect.UI.Utils;

namespace Architect.UI.Widgets;

class Stack : MultiContentWidget
{
    public int Spacing { get; set; }

    public new ObservableCollection<IWidget> Content { get => field; set => SetProperty(ref field, value); }


    public Stack()
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
        widget.OnDetachFromWidget();
        widget.Dispose();
    }

    public void Clear()
    {
        foreach (var widget in Content)
        {
            widget.OnDetachFromWidget();
        }
        Content.Clear();

        }

    public override void OnAttachToWidget(IDrawingContext context)
    {
        base.OnAttachToWidget(context);
        // Attach all the widgets to the context
        var currentX = Position.X;
        foreach (var widget in Content)
        {
            var widgetContext = new DrawingContext(this, widget);
            widget.Position = VerticalAlignment switch
            {
                VerticalAlignment.Center => widget.Position with { Y = Position.Y + AlignmentHelper.Center(widgetContext.Size, widget.Size).Y },
                VerticalAlignment.Bottom => widget.Position with { Y = Position.Y + AlignmentHelper.Bottom(widgetContext.Size, widget.Size).Y },
                _ => widget.Position with { X = currentX }
            };

            widget.OnAttachToWidget(widgetContext);
            currentX += widget.Size.Width + Spacing;
        }
    }

    public override void OnDetachFromWidget() => Clear();

    public override void Draw()
    {
        foreach (var widget in Content)
        {
            widget.Draw();
        }
    }

    public override void Dispose()
    {
        Clear();
        base.Dispose();
    }


}
