using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Architect.Common.Interfaces;
using Cosmos.System.Graphics;

namespace Architect.UI.Base;

public class MultiContentWidget : Widget
{
    /// <summary>
    /// Gets or sets the collection of content widgets.
    /// </summary>
    public new List<IWidget> Content { get => field; set => SetProperty(ref field, value); }

    
    public void Add(Widget widget)
    {
        Content.Add(widget);
        widget.OnAttachToWidget(this);
    }

    public void Remove(IWidget widget)
    {
        Content.Remove(widget);
        widget.Dispose();
    }

    public void Clear(bool dipose = true)
    {
        foreach (var widget in Content)
            Remove(widget);
                
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