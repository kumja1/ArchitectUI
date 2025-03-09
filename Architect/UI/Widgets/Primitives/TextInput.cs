using Size = Architect.Common.Models.Size;
using Vector2 = Architect.Common.Models.Vector2;
using Architect.Common.Interfaces;
using Cosmos.System;
using Architect.Core.Input.Events;
using Architect.Core.Input;
using Architect.UI.Widgets.Base;
using Architect.UI.Widgets.Bindings;
using System.Drawing;

namespace Architect.UI.Widgets.Primitives;

class TextInput : Widget, IFocusableWidget
{

    public bool IsFocused { get; private set; }

    public EventHandler<InputEvent> TextChanged;

    public TextBlock InnerTextBlock
    {
        get => GetProperty<TextBlock>(nameof(InnerTextBlock));
        set => SetProperty(nameof(InnerTextBlock), value);
    }

    public Color BorderColor
    {
        get => GetProperty<Color>(nameof(BorderColor));
        set => SetProperty(nameof(BorderColor), value);
    }

    public int BorderThickness
    {
        get => GetProperty<int>(nameof(BorderThickness));
        set => SetProperty(nameof(BorderThickness), value);
    }

    public int BorderFocusMultiplier { get; set; } = 2;

    private static readonly List<ConsoleKeyEx> _lettersAndNumbers = [..Enum.GetValues(typeof(ConsoleKeyEx))
            .Cast<ConsoleKeyEx>()
            .Where(e => e is not (
              // System and function keys (should not produce text)
                ConsoleKeyEx.Escape or
                ConsoleKeyEx.F1 or
                ConsoleKeyEx.F2 or
                ConsoleKeyEx.F3 or
                ConsoleKeyEx.F4 or
                ConsoleKeyEx.F5 or
                ConsoleKeyEx.F6 or
                ConsoleKeyEx.F7 or
                ConsoleKeyEx.F8 or
                ConsoleKeyEx.F9 or
                ConsoleKeyEx.F10 or
                ConsoleKeyEx.F11 or
                ConsoleKeyEx.F12 or
                ConsoleKeyEx.PrintScreen or
                ConsoleKeyEx.ScrollLock or
                ConsoleKeyEx.PauseBreak or
                ConsoleKeyEx.Tab or
                ConsoleKeyEx.CapsLock or
                ConsoleKeyEx.LShift or
                ConsoleKeyEx.RShift or
                ConsoleKeyEx.LCtrl or
                ConsoleKeyEx.RCtrl or
                ConsoleKeyEx.LAlt or
                ConsoleKeyEx.RAlt or
                ConsoleKeyEx.LWin or
                ConsoleKeyEx.RWin or
                ConsoleKeyEx.Menu or 
                // Navigation keys
                ConsoleKeyEx.Insert or
                ConsoleKeyEx.Home or
                ConsoleKeyEx.PageUp or
                ConsoleKeyEx.End or
                ConsoleKeyEx.PageDown or
                ConsoleKeyEx.UpArrow or
                ConsoleKeyEx.DownArrow or
                ConsoleKeyEx.LeftArrow or
                ConsoleKeyEx.RightArrow or
                // Numeric keypad control keys
                ConsoleKeyEx.NumLock or
                ConsoleKeyEx.NumEnter or
                // Others
                ConsoleKeyEx.Power or
                ConsoleKeyEx.Sleep or
                ConsoleKeyEx.Wake
                ))
              ];


    public TextInput()
    {
        Position = new Vector2(0, 0);
        Size = new Size(100, 30);

        Content = new Border
        {
            OutlineThickness = new Size(BorderThickness, BorderThickness),
            BackgroundColor = BackgroundColor,
            OutlineColor = BorderColor,
            Content = InnerTextBlock
        };

        Bind<TextInput, int>(nameof(BorderThickness))
             .WithBindingDirection(BindingDirection.TwoWay)
            .WithConverter(converter: b => new Size(b, b), backwardConverter: s => s.Width)
            .To((Border)Content, nameof(Border.OutlineThickness));

        Bind<TextInput, Color>(nameof(BorderColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To((Border)Content, nameof(Border.OutlineColor));

        Bind<TextInput, Color>(nameof(BackgroundColor))
            .WithBindingDirection(BindingDirection.TwoWay)
            .To((Border)Content, nameof(BackgroundColor));
    }


    public override void OnAttachToWidget(IWidget parent)
    {
        base.OnAttachToWidget(parent);

        InputManager.Instance.RegisterMouseInput<MouseClickEvent>(this, InputType.MouseClick, OnFocus);
        InputManager.Instance.RegisterMouseInput<MouseClickOutEvent>(this, InputType.MouseClickOut, OnUnfocus);
        InputManager.Instance.RegisterKeyboardInput(this, InputType.KeyboardPress, [ConsoleKeyEx.Escape], OnUnfocus);


        InputManager.Instance.RegisterKeyboardInput<TextInput, KeyboardPressedEvent>(this, InputType.KeyboardPressed, _lettersAndNumbers, OnTextInput);
        InputManager.Instance.RegisterKeyboardInput<TextInput, KeyboardPressEvent>(this, InputType.KeyboardPress, [ConsoleKeyEx.Enter, ConsoleKeyEx.Backspace, ConsoleKeyEx.Delete, ConsoleKeyEx.Spacebar], OnControlKey);

    }

    private void OnUnfocus(object? sender, InputEvent e)
    {
        if (!IsFocused) return;

        BorderThickness /= BorderFocusMultiplier;
        IsFocused = false;
    }

    private void OnFocus(object? sender, MouseClickEvent e)
    {
        BorderThickness *= BorderFocusMultiplier;
        IsFocused = true;
    }

    private void OnTextInput(object? sender, KeyboardPressedEvent e) => InnerTextBlock.Text += e.KeyChar.ToString();


    private void OnControlKey(object? sender, KeyboardPressEvent e)
    {
        switch (e.Key)
        {
            case ConsoleKeyEx.Enter:
                IsFocused = false;
                break;

            case ConsoleKeyEx.Backspace:
                InnerTextBlock.Text = InnerTextBlock.Text[..^1];
                break;

            case ConsoleKeyEx.Delete:
                InnerTextBlock.Text = InnerTextBlock.Text[1..];
                break;

            case ConsoleKeyEx.Spacebar:
                InnerTextBlock.Text += " ";
                break;

        }
    }


    public override void OnDetachFromWidget()
    {
        base.OnDetachFromWidget();
        InputManager.Instance.RemoveInput(this);
    }

    public override void Dispose()
    {
        base.Dispose();
        InnerTextBlock.Dispose();
    }

    bool IFocusableWidget.IsFocused { get => IsFocused; set => IsFocused = value; }
}
