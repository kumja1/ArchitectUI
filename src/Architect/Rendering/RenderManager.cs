using Architect.UI;
using Architect.UI.Widgets;
using Cosmos.Kernel.System.Graphics;


namespace Architect.Rendering;

public sealed class RenderManager(Canvas canvas)
{
    private static RenderManager? _instance;

    public static RenderManager Instance =>
        _instance ?? throw new InvalidOperationException("RenderManager not initialized.");

    public static RenderManager Initialize(Canvas canvas) =>
        _instance ??= new RenderManager(canvas);

    private readonly List<Rect> _dirtyRegions;

    public void Tick()
    {
        if (_dirtyRegions.Count == 0)
            return;

    }

    public void ScheduleRedraw(Widget widget, RenderPriority priority = RenderPriority.Default)
    {
       Rect.Within()
    }
}