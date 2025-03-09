using System.Drawing;
using Architect.Common.Interfaces;
using Cosmos.System.Graphics;

namespace Architect.Core.Rendering;

sealed class RenderManager(Canvas canvas)
{
    private static RenderManager? _instance;

    public static RenderManager Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("RenderManager must be initialized first");
            }
            return _instance;
        }
    }

    public static RenderManager Initialize(Canvas canvas)
    {
        return _instance ??= new RenderManager(canvas);
    }

    private readonly PriorityQueue<IWidget, int> _dirtyWidgets = new();

    private Canvas Canvas { get; init; } = canvas;
    public void Tick()
    {
        DrawMouse();
        if (_dirtyWidgets.Count == 0)
            return;

        while (_dirtyWidgets.Count > 0 && _dirtyWidgets.TryDequeue(out var widget, out _))
        {
            Erase(widget);
            widget.BeginDraw(Canvas);
        }

        _dirtyWidgets.Clear();
    }

    private void DrawMouse()
    {
    }

    public void ScheduleRedraw(IWidget widget)
    {
        if (widget.IsVisible && widget.ZIndex > 0)
        {
            _dirtyWidgets.Enqueue(widget, widget.ZIndex);
        }
    }

    public void Erase(IWidget widget) => Canvas.DrawRectangle(Color.Transparent, widget.Position.X, widget.Position.Y, widget.Size.Width, widget.Size.Height);
}