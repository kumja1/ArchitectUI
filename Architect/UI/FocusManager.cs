using Architect.Common.Interfaces;
using Architect.UI.Widgets.Base;

namespace Architect.UI;

public class FocusManager
{
    private static FocusManager? _instance;
    public static FocusManager Instance => _instance ??= new FocusManager();

    public IFocusableWidget? FocusedWidget { get; private set; }

    public void SetFocus(IFocusableWidget widget)
    {
        if (widget == FocusedWidget)
            return;

        FocusedWidget?.OnUnfocus();
        FocusedWidget = widget;
        FocusedWidget.OnFocus();
    }

    public void RemoveFocus(Widget widget)
    {
        if (FocusedWidget == widget)
        {
            FocusedWidget.OnUnfocus();
            FocusedWidget = null;
        }
    }
}
