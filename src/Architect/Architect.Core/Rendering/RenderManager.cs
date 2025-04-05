using System.Drawing;
using Architect.Common.Interfaces;
using Cosmos.System.Graphics;
using Size = Architect.Common.Models.Size;

namespace Architect.Core.Rendering;

public sealed class RenderManager(Canvas canvas)
{
    private static RenderManager? _instance;

    public static RenderManager Instance =>
        _instance ?? throw new InvalidOperationException("RenderManager not initialized.");

    public static RenderManager Initialize(Canvas canvas) =>
        _instance ??= new RenderManager(canvas);

    private readonly PriorityQueue<IWidget, int> _dirtyWidgets = new();

    private readonly Canvas _canvas = canvas;

    public Size ScreenSize => new((int)_canvas.Mode.Width, (int)_canvas.Mode.Height);

    public void Tick()
    {
        DrawMouse();

        if (_dirtyWidgets.Count == 0)
            return;

        while (_dirtyWidgets.Count > 0 && _dirtyWidgets.TryDequeue(out var widget, out _))
        {
            Erase(widget);
            widget.BeginDraw(_canvas);
        }

        _dirtyWidgets.Clear();
    }

    private void DrawMouse() { }

    public void ScheduleRedraw(IWidget widget)
    {
        if (widget.IsVisible && widget.ZIndex > 0)
        {
            _dirtyWidgets.Enqueue(widget, widget.ZIndex);
        }
    }

    public void Erase(IWidget widget) =>
        Canvas.DrawRectangle(
            Color.Transparent,
            widget.Position.X,
            widget.Position.Y,
            widget.Size.Width,
            widget.Size.Height
        );
}
