using Size = Architect.Common.Models.Size;
using Vector2 = Architect.Common.Models.Vector2;
using Architect.Common.Interfaces;
using Cosmos.System;
using Architect.Core.Input.Events;
using Architect.Core.Input;
using Architect.UI.Base;
using System.Drawing;

namespace Architect.UI.Primitives;

class TextInput : Widget, IFocusableWidget
{

    public bool IsFocused { get; private set; }

    public EventHandler<InputEvent> TextChanged;

    public TextBlock InnerTextBlock
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public Color BorderColor { get => field; set => SetProperty(ref field, value); } = Color.Black;

    public int FocusThickness { get => field; set => SetProperty(ref field, value); } = 2;

    public int FocusMultiplier { get => field; set => SetProperty(ref field, value); } = 2;

    IWidget IWidget.Content { get; set => SetProperty(ref field, value); }

    public TextInput()
    {
        Position = new Vector2(0, 0);
        Size = new Size(100, 30);

        Content = new Border
        {
            OutlineThickness = new Size(FocusThickness, FocusThickness),
            BackgroundColor = BackgroundColor,
            OutlineColor = BorderColor,
            Content = InnerTextBlock
        };
    }


    public override void OnAttachToWidget(IWidget parent)
    {
        base.OnAttachToWidget(parent);

        InputManager.Instance.RegisterMouseInput<MouseClickEvent>(this, InputType.MouseClick, OnMouseClick);
        InputManager.Instance.RegisterMouseInput<MouseClickOutEvent>(this, InputType.MouseClickOut, OnMouseClickOut);

        InputManager.Instance.RegisterKeyboardInput(this, [.. Enum.GetValues(typeof(ConsoleKeyEx)).Cast<ConsoleKeyEx>()], OnTextChanged);

    }

    private void OnMouseClickOut(object? sender, MouseClickOutEvent e)
    {
        FocusThickness /= FocusMultiplier;
        IsFocused = false;
    }

    private void OnMouseClick(object? sender, MouseClickEvent e)
    {
        FocusThickness *= FocusMultiplier;
        IsFocused = true;
    }

    private void OnTextChanged(object sender, InputEvent e)
    {
        if (e is KeyboardEvent keyboardEvent)
        {
            if (keyboardEvent.Type == KeyEvent.KeyEventType.Make)
            {
                if (keyboardEvent.Key == ConsoleKeyEx.Backspace && InnerTextBlock.Text.Length > 0)
                    InnerTextBlock.Text = InnerTextBlock.Text[0..^1];
                else if (keyboardEvent.Key == ConsoleKeyEx.Delete && InnerTextBlock.Text.Length > 0)
                    InnerTextBlock.Text = string.Empty;
                else if (keyboardEvent.Key == ConsoleKeyEx.Enter)
                    IsFocused = false;
                else if (keyboardEvent.Key == ConsoleKeyEx.Spacebar)
                    InnerTextBlock.Text += " ";
                else
                    InnerTextBlock.Text += keyboardEvent.KeyChar.ToString();

            }
        }
    }

    public override void OnDetachFromWidget()
    {
        base.OnDetachFromWidget();
        InputManager.Instance.RemoveInput(this);
    }


    bool IFocusableWidget.IsFocused { get => IsFocused; set => IsFocused = value; }
}
