namespace Architect.UI.Widgets.Layout.Stack;

class VerticalStackPanel : StackPanel
{
    public sealed override StackOrientation Orientation => StackOrientation.Vertical;

    public VerticalStackPanel()
    {
        VerticalAlignment = VerticalAlignment.Stretch;
    }
}
