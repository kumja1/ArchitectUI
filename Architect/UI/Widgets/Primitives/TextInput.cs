using System.Drawing;
using Size = Architect.Common.Models.Size;
using Vector2 = Architect.Common.Models.Vector2;
using Architect.Widgets;
using Architect.Common.Interfaces;
using Cosmos.System;
using Architect.Core.Input.Events;
using Architect.Core.Input;

namespace Architect.UI;

class TextInput : Widget, IFocusable
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
        BackgroundColor = Color.White;
        Position = new Vector2(0, 0);
        Size = new Size(100, 30);

        Content = new Background
        {
            Color = BackgroundColor,
            Size = Size,
            Content = new Border
            {
                OutlineRadius = 5,
                OutlineThickness = new Size(1, 1),
                Position = Position,
                Content = new Background
                {
                    Color = BackgroundColor,
                    Content = new TextBlock
                    {
                        Text = Text,
                        TextColor = Color.Black,
                        WrapText = false,
                    }
                }
            }
        };
    }

    public void SetFocus(bool focus)
    {
        throw new NotImplementedException();
    }

     public override void OnAttachToWidget(IDrawingContext context)
    {
    
        base.OnAttachToWidget(context);
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
