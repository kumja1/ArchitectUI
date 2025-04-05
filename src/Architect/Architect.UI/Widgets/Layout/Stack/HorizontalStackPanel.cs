namespace Architect.UI.Widgets.Layout.Stack;

class HorizontalStackPanel : StackPanel
{
    public sealed override StackOrientation Orientation => StackOrientation.Horizontal;

    public HorizontalStackPanel()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
    }
}
