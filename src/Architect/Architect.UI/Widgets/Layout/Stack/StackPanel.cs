using Architect.UI.Widgets.Base;

namespace Architect.UI.Widgets.Layout.Stack;

class StackPanel : MultiContentWidget
{
    public int Spacing
    {
        get => GetProperty<int>(nameof(Spacing));
        set => SetProperty(nameof(Spacing), value);
    }
}
