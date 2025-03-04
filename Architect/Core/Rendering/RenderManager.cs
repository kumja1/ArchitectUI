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

    private readonly HashSet<IWidget> _dirtWidgets = [];

    private Canvas Canvas { get; init; }  = canvas;
    public void Tick()
    {
        DrawMouse();
        if (_dirtWidgets.Count == 0)
            return;

        foreach (var widget in _dirtWidgets)
        {
            Erase(widget);
            widget.BeginDraw(Canvas);
        }

        _dirtWidgets.Clear();
    }

    private void DrawMouse()
    {
    }

    public void ScheduleRedraw(IWidget widget)
    {
        if (widget.IsVisible && widget.ZIndex > 0 && !_dirtWidgets.Contains(widget))
            _dirtWidgets.Add(widget);
    }

    public void Erase(IWidget widget) => Canvas.DrawRectangle(Color.Transparent, widget.Position.X, widget.Position.Y, widget.Size.Width, widget.Size.Height);



}