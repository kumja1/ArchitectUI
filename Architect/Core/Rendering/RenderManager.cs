using Architect.Common.Interfaces;

namespace Architect.UI;

static class RenderManager
{
    private static readonly HashSet<IWidget> _dirtyWindows = [];

    private static Canvas Canvas { get; set; }

    public static void Tick()
    {
        DrawMouse();

        if (_dirtyWindows.Count == 0)
            return;

        foreach (var window in _dirtyWindows)
        {
            window.RedrawDirtyWidgets();
        }

        _dirtyWindows.Clear();
    }

    public static void Initialize(Canvas canvas)
    {
        Canvas = canvas;
    }

    private static void DrawMouse()
    {
    }

    public static void ScheduleWindowUpdate(IWidget window)
    {
        if (!_dirtyWindows.Contains(window))
        {
            _dirtyWindows.Add(window);
        }
    }

    public static void ClearArea(Vector2 position, Vector2 size) => Canvas.ClearArea(position, size);
}