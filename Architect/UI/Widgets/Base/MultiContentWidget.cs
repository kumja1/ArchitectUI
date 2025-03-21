using Architect.Common.Interfaces;
using Cosmos.System.Graphics;

namespace Architect.UI.Widgets.Base;

public class MultiContentWidget : Widget
{
    /// <summary>
    /// Gets or sets the collection of content widgets.
    /// </summary>
    public new List<IWidget> Content
    {
        get => GetProperty<List<IWidget>>(nameof(Content), defaultValue: []);
        set => SetProperty(nameof(Content), value);
    }

    public void Add(Widget widget)
    {
        Content?.Add(widget);
        widget.OnAttachToWidget(this);
    }

    public void Remove(IWidget widget)
    {
        Content?.Remove(widget);
        widget.Dispose();
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(Content, nameof(Content));
        foreach (var widget in Content)
        {
            widget.Draw(canvas);
        }
    }

    public override void Dispose()
    {
        ArgumentNullException.ThrowIfNull(Content, nameof(Content));
        foreach (var widget in Content)
        {
            widget.Dispose();
        }

        Content.Clear();
        GC.SuppressFinalize(this);
    }
}
