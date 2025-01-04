using Architect.Common.Interfaces;
using Cosmos.System.Graphics;

class RenderManager(Canvas canvas)
{
    private readonly HashSet<IWidget> DirtyWidgets = [];

    private readonly Canvas Canvas = canvas;

    public void Tick()
    {
        if (DirtyWidgets.Count == 0)
            return;

        foreach (var widget in DirtyWidgets)
        {
            widget.MarkDirty(true);
            widget.BeginDraw();
            widget.MarkDirty(false);
        }
        
        DirtyWidgets.Clear();
    }

    public void AddDirtyWidget(IWidget widget) => DirtyWidgets.Add(widget);

    public void RemoveDirtyWidget(IWidget widget) => DirtyWidgets.Remove(widget);
}