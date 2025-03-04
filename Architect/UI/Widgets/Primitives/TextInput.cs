using Size = Architect.Common.Models.Size;
using Vector2 = Architect.Common.Models.Vector2;
using Architect.Common.Interfaces;
using Cosmos.System;
using Architect.Core.Input.Events;
using Architect.Core.Input;
using Architect.UI.Base;

namespace Architect.UI.Primitives;

class TextInput : Widget
{

    public bool IsFocused { get => field; set => SetProperty(ref field, value); }

    public string Text
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public TextInput()
    {
        Text = string.Empty;
        Position = new Vector2(0, 0);
        Size = new Size(100, 30);
    }

    public void SetFocus(bool focus)
    {
        throw new NotImplementedException();
    }

     public override void OnAttachToWidget(IWidget parent)
    {
    
        base.OnAttachToWidget(parent);
        InputManager.Instance.RegisterKeyboardInput(this, [.. Enum.GetValues(typeof(ConsoleKeyEx)).Cast<ConsoleKeyEx>()], OnTextChanged);
        
    }

    private void OnTextChanged(object sender, InputEvent e)
    {
        if (e is KeyboardEvent keyboardEvent)
        {
            if (keyboardEvent.Type == KeyEvent.KeyEventType.Make)
            {
                if (keyboardEvent.Key == ConsoleKeyEx.Backspace && Text.Length > 0)
                {
                    Text = Text[0..^1];
                }
                else if (keyboardEvent.Key == ConsoleKeyEx.Delete && Text.Length > 0)
                {
                    Text = string.Empty;
                }
                else
                {
                    Text += keyboardEvent.Key.ToString();
                }
            }
        }
    }

    public override void OnDetachFromWidget()
    {
        base.OnDetachFromWidget();
        InputManager.Instance.Unregister(this);
    }
}
