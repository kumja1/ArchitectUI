using Architect.Common.Interfaces;
using Architect.UI;
using Cosmos.System.Graphics;

namespace Architect.Core;

static class RenderManager
{
    private static readonly HashSet<IWidget> DirtyWindows = [];

    private static readonly Canvas Canvas;

    public static void Tick()
    {
        DrawMouse();
        
        if (DirtyWindows.Count == 0)
            return;

        foreach (var window in DirtyWindows)
        {
            window.BeginDraw();
        }

        DirtyWindows.Clear();
    }

    private static void DrawMouse()
    {
    }

    
    public static void ScheduleWindowUpdate(Window window)
    {
        if (!DirtyWindows.Contains(window))
        {
            DirtyWindows.Add(window);
        }
    }
}