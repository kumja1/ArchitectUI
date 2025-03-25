namespace Architect.Common.Interfaces;

public interface IFocusableWidget : IWidget
{
    public bool IsFocused { get; set; }

    public void OnFocus();
    public void OnUnfocus();
}
