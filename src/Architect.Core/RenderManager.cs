using Architect.Common.Interfaces;
using System.Collections.Generic;

class RenderManager
{
    private readonly HashSet<IWidget> DirtyWidgets = [];

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