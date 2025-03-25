using Architect.Common.Interfaces;
using Cosmos.System.Graphics;

namespace Architect.UI.Widgets.Base;

public class MultiContentWidget : Widget
{
    /// <summary>
    /// Gets or sets the collection of content widgets.
    /// </summary>
    public new List<IWidget>? Content
    {
        get => GetProperty<List<IWidget>>(nameof(Content), defaultValue: null);
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
}
